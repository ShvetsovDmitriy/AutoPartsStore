using AutoPartsStore.Model.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AutoPartsStore.Services.Product
{
    public interface IProductService
    {
        Task<List<Products>> GetProductsAsync(string productName, int? productTypeId, bool? inStockOnly, string sortBy);
    }

    public class ProductService : IProductService
    {
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;

        public ProductService(MyDbContext context, IMemoryCache cache, ILogger<ProductService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                        _ => productsQuery,
                    };
                }

                products = await productsQuery.Include(p => p.ProductType).ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                _cache.Set(cacheKey, products, cacheOptions);
            }
            if (products == null || !products.Any())
            {
                return null;

            }
            else
            {
                return products;
            }
        }
    }
}

