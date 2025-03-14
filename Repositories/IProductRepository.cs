﻿using GeekStore.API.Models.Domains;
using GeekStore.API.Models.DTOs;

namespace GeekStore.API.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(string? queryOn, string? queryFilter, string? sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
        Task<Product?> DeleteAsync(Guid id);
    }
}
