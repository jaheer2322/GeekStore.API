using System.Text.Json;
using AutoMapper;
using GeekStore.API.Models.DTOs;
using GeekStore.API.Models.Domains;
using GeekStore.API.Services.Interfaces;
using GeekStore.API.Repositories.Interfaces;

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

        public async Task<RecommendationsDto?> GetRecommendationAsync(string query)
        {
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

            You will receive two things:
            1. A user query describing their PC needs.
            2. A JSON object of available PC components grouped by category. Each category has exactly 5 products, each with a unique GUID.

            Your task:
            - Return an array of EXACTLY TWO valid PC builds.
            - Each build must be a flat JSON object: each key is a category (as given in input), and each value is a GUID (copied exactly from input).
            - DO NOT invent or add new categories not present in the input — return only the categories exactly as-is.
            - DO NOT change, merge, or generate GUIDs. Copy the GUIDs from input exactly.
            - Values must ONLY be GUID strings — no objects, names, or explanations.
            - Output must be a plain JSON array with two builds. No text before or after.
            - If a valid build cannot be created based on input or query, return an empty array: `[]`.

            Example format:
            [
              {
                "Category1": "GUID",
                "Category2": "GUID",
                "Category3": "GUID"
              },
              {
                "Category1": "GUID",
                "Category2": "GUID",
                "Category3": "GUID"
              }
            ]
            
            - MUST have compatible CPU and Motherboard sockets (see description for sockets),
            - Make sure that the builds have low bottleneck among its components.
            - Avoid giving same parts for both builds.
            - Do NOT include any explanations or additional formatting. Return only the JSON array.
            """;

            var llmResultJson = await _llmService.GenerateRecommendationAsync(systemPrompt, query, availableProductsJson);

            // Deserialize LLM output
            var rawBuilds = JsonSerializer.Deserialize<List<Dictionary<string, Guid>>>(llmResultJson);
            if (rawBuilds == null) return null;

            // Map to BuildDto
            var recommendedBuilds = new List<BuildDto>();

            foreach (var build in rawBuilds)
            {
                if (build.Keys.Except(similarProducts.Keys).Any())
                {
                    throw new Exception("LLM returned categories that were not provided.");
                }

                var parts = new Dictionary<string, RecommendedProductDto>();

                foreach (var (category, productId) in build)
                {
                    var product = similarProducts[category].FirstOrDefault(p => p.Id == productId);
                    
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {productId} not found in category {category}.");
                    }

                    parts[category] = _mapper.Map<RecommendedProductDto>(product);
                }

                recommendedBuilds.Add(new BuildDto { Parts = parts });
            }

            var message = (recommendedBuilds.Count() == 0)
                ? "Sorry, we currently cannot recommend a PC build for your query. Please make sure that the query is related to PC recommendation."
                : "Here are the recommended PC builds based on your query among our best available components. Please note that currently there are no suitable products for the categories that are not mentioned in the recommendations";

            // Wrap in DTO and return
            return new RecommendationsDto
            {
                Message = message,
                Builds = recommendedBuilds
            };
        }
    }
}
