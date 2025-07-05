using GeekStore.API.Data;
using GeekStore.API.Models.Domains;
using GeekStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pgvector;

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
            bool exists = await _geekStoreDbContext.Products
                .AnyAsync(p => p.Name == product.Name
                            && p.TierId == product.TierId
                            && p.CategoryId == product.CategoryId);

            if (exists)
            {
                return null;
            }

            await _geekStoreDbContext.Products.AddAsync(product);
            await _geekStoreDbContext.SaveChangesAsync();

            var createdProduct = _geekStoreDbContext.Products
                .AsNoTracking()
                .Include("Tier")
                .Include("Category")
                .FirstOrDefault(_product => _product.Id == product.Id);
            
            if(createdProduct == null)
            {
                return null;
            }

            return createdProduct;
        }

        public async Task<List<Product>> GetAllAsync(string? column, string? query, 
            string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var products = _geekStoreDbContext.Products
                .AsNoTracking()
                .Include("Tier")
                .Include("Category");

            // Filtering
            if (string.IsNullOrWhiteSpace(column) == false && string.IsNullOrWhiteSpace(query) == false)
            {
                query = query.ToLower();
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
                        "category" => products.OrderByDescending(product => product.Category.Name),
                        _ => products
                    };
                }
                else
                {
                    products = sortBy.ToLower() switch
                    {
                        "name" => products.OrderBy(product => product.Name),
                        "price" => products.OrderBy(product => product.Price),
                        "category" => products.OrderBy(product => product.Category.Name),
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
            return await _geekStoreDbContext.Products
                .AsNoTracking()
                .Include("Tier")
                .Include("Category")
                .FirstOrDefaultAsync(product => product.Id == id);
        }
        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var product = await _geekStoreDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            bool exists = await _geekStoreDbContext.Products
                .AnyAsync(p => p.Name == updatedProduct.Name
                            && p.TierId == updatedProduct.TierId
                            && p.CategoryId == updatedProduct.CategoryId);

            if (exists || product == null)
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

        public async Task SaveEmbeddingAsync(Guid productId, Vector embedding)
        {
            var product = await _geekStoreDbContext.Products.FindAsync(productId);

            if (product == null)
                throw new KeyNotFoundException($"Product not found for id {productId}");

            product.Embedding = embedding;
            await _geekStoreDbContext.SaveChangesAsync();
        }
        public async Task<Dictionary<string, List<Product>>> GetSimilarProductsAsync(Vector inputVector)
        {
            var categories = await _geekStoreDbContext.Categories.AsNoTracking().ToListAsync();
            var result = new Dictionary<string, List<Product>>();

            // Giving more number of CPU, GPU and Motherboard options as they are the core components  
            var coreComponents = new List<string> { "CPU", "Motherboard", "GPU" };

            foreach (var category in categories)
            {
                var limit = coreComponents.Contains(category.Name) ? 10 : 5;

                var topProducts = await _geekStoreDbContext.Products
                    .FromSqlRaw(@"
                               SELECT * FROM ""Products""  
                               WHERE ""CategoryId"" = {0} AND (""Embedding"" <=> {1} <= 0.7) -- Low confidence rejection
                               ORDER BY (""Embedding"" <=> {1})  
                               LIMIT {2}", category.Id, inputVector, limit)
                    .ToListAsync();
  
                if (topProducts.Any())
                {
                    result[category.Name] = topProducts;
                }
            }
            return result;
        }
    }
}
