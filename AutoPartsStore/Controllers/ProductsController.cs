using AutoPartsStore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace AutoPartsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;


        public ProductsController(MyDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
      
        
        [HttpGet]
   
        public async Task<IActionResult> GetProducts([FromQuery] string productName, [FromQuery] int? productTypeId, [FromQuery] bool? inStockOnly, [FromQuery] string sortBy)
        {

            var cacheKey = $"{productName}_{productTypeId}_{inStockOnly}_{sortBy}";
            if (!_cache.TryGetValue(cacheKey, out List<Products> products))
            {
                IQueryable<Products> productsQuery = _context.Products.Include(p => p.ProductType);


            if (!string.IsNullOrEmpty(productName))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.Contains(productName));
            }
        
            if (productTypeId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductType.ProductTypeId == productTypeId);
            }
            
            if (inStockOnly.HasValue && inStockOnly.Value)
            {
                productsQuery = productsQuery.Where(p => p.AvailableQuantity > 0);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price_asc":
                        productsQuery = productsQuery.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        productsQuery = productsQuery.OrderByDescending(p => p.Price);
                        break;
                    default:

                        break;
                }
            }

                products = await productsQuery.ToListAsync();

                // Кэшируем полученные данные
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) 
                };
                _cache.Set(cacheKey, products, cacheOptions);
            }

            return Ok(products);
           
        }

    }
}