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
               You are an expert PC builder. Given a user query and a JSON object of available PC components grouped by category (each category has exactly 3 products), your job is to return an array of exactly two valid PC builds.

               Each build must:
               - Be a flat JSON object.
               - Strictly contain the categories present in the input JSON only as keys.
               - Do not create any new categories or modify existing ones even if the user infers a category that is not present in input JSON.
               - Have values as only product IDs (GUIDs) exactly as given in the input.
               - Not include objects, additional keys, or any formatting — just the product ID string as the value.
               - Make sure the products within a build are compatible and no bottlenecks are present.
               
               Example build format (do not include explanations):
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

               Return only the array. No explanations, no markdown, no text before or after.
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

            // Wrap in DTO and return
            return new RecommendationsDto
            {
                Builds = recommendedBuilds
            };
        }
    }
}
