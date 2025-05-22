using GeekStore.API.Models.Domains;
using GeekStore.API.Repositories;

namespace GeekStore.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Product?> CreateAsync(Product product)
        {
            var createdProduct = await _productRepository.CreateAsync(product);
            return createdProduct;
        }
    }
}
