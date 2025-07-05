using GeekStore.API.Models.Domains;
using GeekStore.API.Repositories.Interfaces;
using GeekStore.API.Services.Interfaces;

namespace GeekStore.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IEmbeddingQueue _embeddingQueue;
        public ProductService(IProductRepository productRepository, IEmbeddingQueue embeddingQueue)
        {
            _productRepository = productRepository;
            _embeddingQueue = embeddingQueue;
        }
        public async Task<Product?> CreateAsync(Product product)
        {
            var createdProduct = await _productRepository.CreateAsync(product);

            if (createdProduct == null)
            {
                return null;
            }

            SaveEmbedding(createdProduct);
            return createdProduct;
        }
        public async Task<List<Product>> CreateMultipleAsync(List<Product> products)
        {
            var createdProducts = new List<Product>();

            foreach (var product in products)
            {
                var createdProduct = await CreateAsync(product);
                if (createdProduct == null)
                {
                    return createdProducts;
                }
                createdProducts.Add(createdProduct);
            }

            return createdProducts;
        }

        public async Task<Product?> UpdateAsync(Guid id, Product productDetails)
        {
            var updatedProduct = await _productRepository.UpdateAsync(id, productDetails);

            if (updatedProduct == null)
            {
                return null;
            }

            SaveEmbedding(updatedProduct);
            return updatedProduct;
        }

        private void SaveEmbedding(Product createdProduct)
        {
            string baseString = !string.IsNullOrWhiteSpace(createdProduct.Description)
                ? createdProduct.Description 
                : createdProduct.Name;

            string embeddingString = $"{baseString} {createdProduct.Tier.Name} {createdProduct.Category.Name}.";

            if (!string.IsNullOrWhiteSpace(createdProduct.Review))
            {
                embeddingString += $" {createdProduct.Review}";
            }

            _embeddingQueue.Enqueue(createdProduct.Id, embeddingString);
        }
    }
}
