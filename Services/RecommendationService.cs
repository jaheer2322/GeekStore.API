using System.Text.Json;
using AutoMapper;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Services.Interfaces;
using GeekStore.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace GeekStore.API.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IProductRepository _productRepository;
        private readonly ILLMService _llmService;
        private readonly IMapper _mapper;

        public RecommendationService(IEmbeddingService embeddingService, IProductRepository productRepository, ILLMService llmService, IMapper mapper)
        {
            _embeddingService = embeddingService;
            _productRepository = productRepository;
            _llmService = llmService;
            _mapper = mapper;
        }

        public async Task<RecommendationsDto?> GetRecommendationAsync(RecommendationQueryDto queryDTO)
        {
            string query = queryDTO.Query.Trim();

            // Embed the query
            var embeddedQuery = await _embeddingService.GenerateEmbeddingAsync(query);

            // Get top 3 similar products from each category
            var similarProducts = await _productRepository.GetSimilarProductsAsync(embeddedQuery);

            // Serialize for LLM input
            var compactProducts = new Dictionary<string, List<CompactProductDto>>();

            foreach(var (category, products) in similarProducts)
            {
                compactProducts[category] = _mapper.Map<List<CompactProductDto>>(products);
            }

            var availableProductsJson = JsonSerializer.Serialize(compactProducts);


            // Get LLM recommendation result (JSON string)
            var systemPrompt = """
               You are an expert PC builder.

               You will receive:
               1. A user query describing PC requirements.
               2. A JSON object of available components grouped by category. Each category contains 5 products, each with a unique GUID.

               Your task:
               - Return up to 2 valid PC builds as a JSON array.
               - Each build must be a flat JSON object with keys as category names (exactly as given), and values as GUID strings (copied exactly from input).
               - Use only provided categories. Do not add, rename, or merge categories.
               - Do not invent or modify GUIDs.
               - Think about each build individually one after the other and you are free to use the same parts for both but with atleast 2 parts different if query requires.

               Constraints:
               - A build is valid **only if the CPU and Motherboard have the same socket type**, which is mentioned at the start of their `description` field (e.g., "Socket: AM5"). If no valid builds are possible, return `[]`.
               - Prioratize socket compatibility over anything else. Each decision you make should be based on socket compatibility first, then performance, and then price.
               - Select parts that best match the user's query.
               - Ensure low bottleneck across components.
               - Return only the JSON array. 
               - No explanations or text before/after.
               - Output should be pure JSON without any additional text or formatting.

               FInal Checks:
               - Make sure the data you return in the <think> tag is as if you are a PC expert talking to the user directly.
               - After everything once again check for socket compatibility between Motherboard and CPU.
               - Also check whether the guids you return are exactly as in the available product guids.

               Example:
               [
                 {
                   "Category1": "GUID (exact GUID as in the available products)",
                   "Category2": "GUID (exact GUID as in the available products)",
                   ...
                 },
                 {
                   "Category1": "GUID (exact GUID as in the available products)",
                   "Category2": "GUID (exact GUID as in the available products)",
                   ...
                 }
               ]
           """;

            var llmResultJson = await _llmService.GenerateRecommendationAsync(systemPrompt, query, availableProductsJson);

            // Process the LLM output
            // 1. Extract <think>...</think> content (if present)
            string? think = null;
            string? jsonArray = null;

            const string thinkStart = "<think>";
            const string thinkEnd = "</think>";

            int thinkStartIdx = llmResultJson.IndexOf(thinkStart, StringComparison.OrdinalIgnoreCase);
            int thinkEndIdx = llmResultJson.IndexOf(thinkEnd, StringComparison.OrdinalIgnoreCase);

            if (thinkStartIdx != -1 && thinkEndIdx != -1 && thinkEndIdx > thinkStartIdx)
            {
                int contentStart = thinkStartIdx + thinkStart.Length;
                think = llmResultJson.Substring(contentStart, thinkEndIdx - contentStart).Trim();

                // The JSON array should be after </think>
                int jsonStart = llmResultJson.IndexOf('[', thinkEndIdx);
                if (jsonStart != -1)
                {
                    jsonArray = llmResultJson.Substring(jsonStart).Trim();
                }
            }
            else
            {
                // No <think> tag, assume the whole string is the JSON array
                int jsonStart = llmResultJson.IndexOf('[');
                if (jsonStart != -1)
                {
                    jsonArray = llmResultJson.Substring(jsonStart).Trim();
                }
                else
                {
                    jsonArray = llmResultJson.Trim();
                }
            }

            // 2. Deserialize the JSON array
            var rawBuilds = JsonSerializer.Deserialize<List<Dictionary<string, Guid>>>(jsonArray);
            if (rawBuilds == null) return null;

            // 3. Map to BuildDto (your existing logic)
            var recommendedBuilds = new List<BuildDto>();
            foreach (var build in rawBuilds)
            {
                if (build.Keys.Except(similarProducts.Keys).Any())
                    throw new Exception("LLM returned categories that were not provided.");

                var parts = new Dictionary<string, RecommendedProductDto>();
                double totalPrice = 0;

                foreach (var (category, productId) in build)
                {
                    var product = similarProducts[category].FirstOrDefault(p => p.Id == productId);
                    if (product == null)
                        throw new Exception($"Product with ID {productId} not found in category {category}.");

                    parts[category] = _mapper.Map<RecommendedProductDto>(product);
                    totalPrice += product.Price;
                }

                recommendedBuilds.Add(new BuildDto { Parts = parts, totalBuildPrice = Math.Round(totalPrice, 2) });
            }

            // 4. Compose the message
            var message = (recommendedBuilds.Count == 0)
                ? "Sorry, we currently cannot recommend a PC build for your query. Please make sure that the query is related to PC recommendation."
                : "Here are the recommended PC builds based on your query among our best available components.";

            // 5. Return DTO
            return new RecommendationsDto
            {
                Message = message,
                Builds = recommendedBuilds,
                Explanation = (queryDTO.IsExplanationNeeded && string.IsNullOrWhiteSpace(think)) ? null : think
            };
        }
    }
}
