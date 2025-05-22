using GeekStore.API.Models.Domains;

namespace GeekStore.API.Services
{
    public interface IProductService
    {
        Task<Product?> CreateAsync(Product product);
    }
}
