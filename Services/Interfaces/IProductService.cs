using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;

namespace GeekStore.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product?> CreateAsync(Product product);
        Task<List<Product>?> CreateMultipleAsync(List<Product> products);
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
    }
}
