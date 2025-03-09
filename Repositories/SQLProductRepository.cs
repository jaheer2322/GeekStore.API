using GeekStore.API.Data;
using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GeekStore.API.Repositories
{
    public class SQLProductRepository : IProductRepository
    {
        private readonly GeekStoreDbContext _geekStoreDbContext;

        public SQLProductRepository(GeekStoreDbContext geekStoreDbContext)
        {
            _geekStoreDbContext = geekStoreDbContext;
        }

        public async Task<Product?> CreateAsync(Product product)
        {
            await _geekStoreDbContext.Products.AddAsync(product);
            await _geekStoreDbContext.SaveChangesAsync();

            var createdProduct = _geekStoreDbContext.Products.Include("Tier").Include("Category").FirstOrDefault(_product => _product.Id == product.Id);
            if(createdProduct == null)
            {
                return null;
            }

            return createdProduct;
        }
        public async Task<List<Product>> GetAllAsync(string? column, string? query, 
            string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var products = _geekStoreDbContext.Products.Include("Tier").Include("Category");

            // Filtering
            if (string.IsNullOrWhiteSpace(column) == false && string.IsNullOrWhiteSpace(query) == false)
            {
                products = column.ToLower() switch
                {
                    "name" => products.Where(product => product.Name.ToLower().Contains(query)),

                    "tier" => products = products.Where(product => product.Tier.Name.ToLower() == query),

                    "category" => products.Where(product => product.Category.Name.ToLower() == query),

                    _ => products
                };
            }

            // Sorting
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                
                if (isAscending == false)
                {
                    products = sortBy.ToLower() switch
                    {
                        "name" => products.OrderByDescending(product => product.Name),
                        "price" => products.OrderByDescending(product => product.Price),
                        _ => products
                    };
                }
                else
                {
                    products = sortBy.ToLower() switch
                    {
                        "name" => products.OrderBy(product => product.Name),
                        "price" => products.OrderBy(product => product.Price),
                        _ => products
                    };
                }
            }

            // Pagination
            var skippableProducts = (pageNumber - 1) * pageSize;
            
            return await products.Skip((int)skippableProducts).Take((int)pageSize).ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(Guid id)
        {   
            return await _geekStoreDbContext.Products.Include("Tier").Include("Category").FirstOrDefaultAsync(product => product.Id == id);
        }
        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var product = await _geekStoreDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            
            if (product == null)
                return null;

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.TierId = updatedProduct.TierId;
            product.CategoryId = updatedProduct.CategoryId;

            await _geekStoreDbContext.SaveChangesAsync();
            
            product = await GetByIdAsync(product.Id);

            return product;
        }
        public async Task<Product?> DeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);

            if (product == null)
                return null;
            
            _geekStoreDbContext.Products.Remove(product);
            await _geekStoreDbContext.SaveChangesAsync();
            
            return product;
        }
    }
}
