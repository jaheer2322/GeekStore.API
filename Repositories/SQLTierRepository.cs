using GeekStore.API.Data;
using GeekStore.API.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace GeekStore.API.Repositories
{
    public class SQLTierRepository : ITierRepository
    {
        private readonly GeekStoreDbContext _dbContext;

        public SQLTierRepository(GeekStoreDbContext geekStoreDbContext)
        {
            _dbContext = geekStoreDbContext;
        }
        public async Task<Tier?> CreateAsync(Tier tier)
        {
            await _dbContext.AddAsync(tier);
            await _dbContext.SaveChangesAsync();

            var createdTier = await _dbContext.Tiers.FirstOrDefaultAsync(_tier => _tier.Id == tier.Id);
            return tier;
        }
        public async Task<List<Tier>> GetAllAsync()
        {
            return await _dbContext.Tiers.ToListAsync();
        }
        public async Task<Tier?> GetByIDAsync(Guid id)
        {
            return await _dbContext.Tiers.FirstOrDefaultAsync(tier => tier.Id == id);
        }
        public async Task<Tier?> UpdateAsync(Guid id, Tier tier)
        {
            var tierDomainModel = await GetByIDAsync(id);
            
            if (tierDomainModel == null) 
                return null;

            tierDomainModel.Name = tier.Name;
            await _dbContext.SaveChangesAsync();

            return tierDomainModel;
        }
        public async Task<Tier?> DeleteAsync(Guid id)
        {
            var tier = await GetByIDAsync(id);

            if (tier == null)
                return null;

            _dbContext.Tiers.Remove(tier);
            await _dbContext.SaveChangesAsync();

            return tier;
        }
    }
}
