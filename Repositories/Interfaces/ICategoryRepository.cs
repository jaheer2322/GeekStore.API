using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;

namespace GeekStore.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> CreateAsync(Category category);
        Task<Category?> GetByIdAsync(Guid id);
        Task<List<Category>> GetAllAsync();
        Task<Category?> UpdateAsync(Guid id, Category updatedCategory);
        Task<Category?> DeleteAsync(Guid id);
    }
}
