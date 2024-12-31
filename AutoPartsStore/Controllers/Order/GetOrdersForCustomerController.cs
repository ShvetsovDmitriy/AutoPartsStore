using Microsoft.AspNetCore.Mvc;
using AutoPartsStore.Services.Order;

namespace AutoPartsStore.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerOrderService _customerOrderService;

        public CustomersController(CustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService ?? throw new ArgumentNullException(nameof(customerOrderService));
        }

        [HttpGet]
        [Route("api/orders/customer")]
        public async Task<IActionResult> GetOrdersForCustomer([FromQuery] string fullName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var orders = await _customerOrderService.GetOrdersForCustomerAsync(fullName, startDate, endDate);

                if (orders == null || !orders.Any())
                {
                    return NotFound("Заказы для указанного клиента не найдены.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка при получении заказов по ФИО клиента: {ex.Message}");
            }
        }
    }
}
