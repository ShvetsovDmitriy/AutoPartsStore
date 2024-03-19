using AutoPartsStore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Serilog;


namespace AutoPartsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly ILogger<OrdersController> _logger;


        public OrdersController(MyDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
           
        }  

        

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] Orders orders)
        {
            try
            {
                // Проверка наличия товара на складе и уменьшение количества
                foreach (var orderItem in orders.OrderItems)
                {
                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    if (product == null || product.AvailableQuantity < orderItem.Quantity)
                    {
                        return BadRequest($"Товар с id {orderItem.ProductId} недоступен или его недостаточное количество на складе.");
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

                return Ok("Заказ успешно создан.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при создании заказа");
                return StatusCode(500, $"Произошла ошибка при создании заказа: {ex.Message}");
            }
        }
    }
}
