using GeekStore.API.Models.Domains;
using Pgvector;

namespace GeekStore.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(string? queryOn, string? queryFilter, string? sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
        Task<Product?> DeleteAsync(Guid id);
        Task SaveEmbeddingAsync(Guid productId, Vector embedding);
        Task<Dictionary<string, List<Product>>> GetSimilarProductsAsync(Vector inputVector);
    }
}
