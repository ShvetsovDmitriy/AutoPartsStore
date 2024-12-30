using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AutoPartsStore.Services
{
    public interface ICustomerInfoService
    {
        Task<IActionResult> GetCustomerByIdAsync(int customerId);
        Task<IActionResult> GetCustomerByPhoneNumberAsync(string phoneNumber);
    }
    public class CustomerInfoService : ICustomerInfoService
    {
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CustomerInfoService> _logger;

        public CustomerInfoService(MyDbContext context, IMemoryCache cache, ILogger<CustomerInfoService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var cacheKey = $"Customer_{customerId}";
                if (_cache.TryGetValue(cacheKey, out Customer cachedCustomer))
                {
                    return new OkObjectResult(cachedCustomer);
                }

                var customer = await _context.Customers.FindAsync(customerId);

                if (customer == null)
                {
                    return new NotFoundResult();
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, customer, cacheOptions);

                return new OkObjectResult(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении клиента по ID");
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> GetCustomerByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                var cacheKey = $"Customer_Phone_{phoneNumber}";
                if (_cache.TryGetValue(cacheKey, out string customerFullName))
                {
                    return new OkObjectResult(customerFullName);
                }

                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

                if (customer == null)
                {
                    return new NotFoundObjectResult("Клиент с указанным номером телефона не найден.");
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, customer.FullName, cacheOptions);

                return new OkObjectResult(customer.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении клиента по номеру телефона");
                return new StatusCodeResult(500);
            }
        }
    }
}
