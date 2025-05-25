using System.Reflection.Metadata.Ecma335;
using GeekStore.API.Data;
using GeekStore.API.Models.Domains;
using GeekStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekStore.API.Repositories
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private readonly GeekStoreDbContext _dbContext;
        public SQLCategoryRepository(GeekStoreDbContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task<Category?> CreateAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var createdCategory = await _dbContext.Categories.FirstOrDefaultAsync(cat => cat.Id == category.Id);
            return createdCategory;
        }
        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == id);
        }
        public async Task<List<Category>> GetAllAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> UpdateAsync(Guid id, Category updatedCategory)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
                return null;
            category.Name = updatedCategory.Name;
            await _dbContext.SaveChangesAsync();

            return category;
        }
        public async Task<Category?> DeleteAsync(Guid id)
        {
            var deletedCategory = await GetByIdAsync(id);
            
            if (deletedCategory==null)
                return null;
        
            _dbContext.Categories.Remove(deletedCategory);
            await _dbContext.SaveChangesAsync();

            return deletedCategory;
        }
    }
}
