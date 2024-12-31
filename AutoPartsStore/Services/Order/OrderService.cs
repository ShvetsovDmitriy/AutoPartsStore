using AutoPartsStore.Model.Order;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AutoPartsStore.Services.Order
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Orders orders);
    }
    public class OrderService : IOrderService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(MyDbContext context, ILogger<OrderService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> CreateOrderAsync(Orders orders)
        {
            try
            {

                if (orders.OrderItems == null || !orders.OrderItems.Any())
                {
                    return $"Товар не найден.";
                }
                foreach (var orderItem in orders.OrderItems)
                {
                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    if (product == null || product.AvailableQuantity < orderItem.Quantity)
                    {
                        return $"Товар с id {orderItem.ProductId} недоступен или его недостаточное количество на складе.";
                    }

                    product.AvailableQuantity -= orderItem.Quantity;
                }

                var newOrder = new Orders
                {
                    CustomerId = orders.CustomerId,
                    OrderDate = DateTime.Now,
                    OrderItems = orders.OrderItems
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return "Заказ успешно создан.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при создании заказа");
                throw; // Пробросим исключение, чтобы обработать его в контроллере
            }
        }
    }
}

