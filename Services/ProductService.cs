using GeekStore.API.Models.Domains;
using GeekStore.API.Repositories;

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

            if(createdProduct == null)
            {
                return null;
            }

            string embeddingString = $"Name: {createdProduct.Name}. " +
                      $"Description: {createdProduct.Description}. " +
                      $"Category: {createdProduct.Category.Name}, Tier: {createdProduct.Tier.Name}. " +
                      $"Price: ${createdProduct.Price}, Quantity in stock: {createdProduct.Quantity}.";

            _embeddingQueue.Enqueue(createdProduct.Id, embeddingString);

            return createdProduct;
        }
    }
}
