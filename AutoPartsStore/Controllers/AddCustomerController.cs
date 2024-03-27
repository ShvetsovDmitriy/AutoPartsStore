using AutoPartsStore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AutoPartsStore.Controllers
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
