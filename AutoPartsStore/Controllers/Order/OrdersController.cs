using AutoPartsStore.Model.Order;
using AutoPartsStore.Services.Order;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] Orders orders)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(orders);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка при создании заказа: {ex.Message}");
            }
        }
    }
}
