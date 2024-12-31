using AutoPartsStore.Services.Customer;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAddController : ControllerBase
    {
        private readonly ICustomerAddService _customerAddService;

        public CustomerAddController(ICustomerAddService customerAddService)
        {
            _customerAddService = customerAddService ?? throw new ArgumentNullException(nameof(customerAddService));
        }

        [HttpPost]
        public Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            return _customerAddService.AddCustomerAsync(customer);
        }
    }
}
