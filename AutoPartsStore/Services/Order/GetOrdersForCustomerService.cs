using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AutoPartsStore.Model.Order;

namespace AutoPartsStore.Services.Order
{
    public class CustomerOrderService
    {
        private readonly MyDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CustomerOrderService> _logger;

        public CustomerOrderService(MyDbContext context, IMemoryCache cache, ILogger<CustomerOrderService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Orders>> GetOrdersForCustomerAsync(string fullName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var cacheKey = $"Orders_{fullName}_{startDate}_{endDate}";
                if (_cache.TryGetValue(cacheKey, out List<Orders>? cachedOrders))
                {
                    return cachedOrders;
                }

                var orders = await _context.Orders
                    .Join(
                        _context.Customers,
                        order => order.CustomerId,
                        customer => customer.CustomerId,
                        (order, customer) => new { Order = order, Customer = customer }
                    )
                    .Where(o => o.Customer.FullName == fullName && o.Order.OrderDate >= startDate && o.Order.OrderDate <= endDate)
                    .OrderBy(o => o.Order.OrderDate)
                    .Select(o => new Orders { OrderId = o.Order.OrderId, OrderDate = o.Order.OrderDate })
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return null;
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                _cache.Set(cacheKey, orders, cacheOptions);

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении заказов по ФИО клиента");
                throw; // Пробрасываем исключение дальше для обработки в контроллере
            }
        }
    }
}
