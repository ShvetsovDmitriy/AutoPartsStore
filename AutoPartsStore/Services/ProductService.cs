
using AutoPartsStore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPartsStore.Services
{
    public interface IProductService
    {
        Task<List<Products>> GetProductsAsync(string productName, int? productTypeId, bool? inStockOnly, string sortBy);
    }

    public class ProductService : IProductService
    {
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductService(MyDbContext context, IMemoryCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<List<Products>> GetProductsAsync(string productName, int? productTypeId, bool? inStockOnly, string sortBy)
        {
            var cacheKey = $"{productName}_{productTypeId}_{inStockOnly}_{sortBy}";
            if (!_cache.TryGetValue(cacheKey, out List<Products>? products))
            {
                IQueryable<Products> productsQuery = _context.Products;

                if (!string.IsNullOrEmpty(productName))
                {
                    productsQuery = productsQuery.Where(p => p.ProductName.Contains(productName));
                }

                if (productTypeId.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.ProductTypeId == productTypeId);
                }

                if (inStockOnly.GetValueOrDefault())
                {
                    productsQuery = productsQuery.Where(p => p.AvailableQuantity > 0);
                }

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    productsQuery = sortBy.ToLower() switch
                    {
                        "price_asc" => productsQuery.OrderBy(p => p.Price),
                        "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                        _ => productsQuery, // No sorting
                    };
                }

                products = await productsQuery.Include(p => p.ProductType).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                _cache.Set(cacheKey, products, cacheOptions);
            }

            return products;
        }
    }
}

