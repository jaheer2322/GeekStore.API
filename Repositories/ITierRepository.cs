using GeekStore.API.Models.Domains;

namespace GeekStore.API.Repositories
{
    public interface ITierRepository
    {
        Task<List<Tier>> GetAllAsync();
        Task<Tier?> GetByIDAsync(Guid id);
        Task<Tier?> CreateAsync(Tier tier);
        Task<Tier?> UpdateAsync(Guid id, Tier tier);
        Task<Tier?> DeleteAsync(Guid id);
    }
}
