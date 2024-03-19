using System.Text.RegularExpressions;
using AutoPartsStore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace AutoPartsStore.Controllers


{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly ILogger<CustomersController> _serilogLogger;
        private readonly IMemoryCache _cache;
        public CustomersController(MyDbContext context, ILogger<CustomersController> logger, IMemoryCache cache)
        {
            _context = context;
            _serilogLogger = logger;
            _cache = cache;
        }

        [HttpPost]
        [ActionName(nameof(GetCustomerById))]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool isRussianFullName = IsRussianFullName(customer.FullName);
                bool isValidPhoneNumber = IsValidPhoneNumber(customer.PhoneNumber);

                if (!isRussianFullName && !isValidPhoneNumber)
                {
                    return BadRequest(new
                    {
                        FullNameError = "ФИО должно быть только на русском языке.",
                        PhoneNumberError = "Номер телефона должен быть в 10-тизначном формате."
                    });
                }
                else if (!isRussianFullName)
                {
                    return BadRequest("ФИО должно быть только на русском языке.");
                }
                else if (!isValidPhoneNumber)
                {
                    return BadRequest("Номер телефона должен быть в 10-тизначном формате.");
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
                _serilogLogger.LogError(ex, "Произошла ошибка при добавлении клиента");
                return StatusCode(500, $"Произошла ошибка при добавлении клиента: {ex.Message}");
            }
        }
        private bool IsRussianFullName(string? fullName)
        {

            if (fullName == null)
            {
                return false; 
            }

            if (Regex.IsMatch(fullName, @"^[А-Яа-яЁё\s]+$"))
            {
                return true;
            }

            return false;
        }

        private bool IsValidPhoneNumber(string? phoneNumber)
        {
            return phoneNumber != null && phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit);
        }


        [HttpGet("{CustomerId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetCustomerById(int CustomerId)
        {
            var cacheKey = $"Customer_{CustomerId}";
            if (_cache.TryGetValue(cacheKey, out Customer customer))
            {
                return Ok(customer);
            }

            customer = await _context.Customers.FindAsync(CustomerId);

            if (customer == null)
            {
                return NotFound();
            }
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            _cache.Set(cacheKey, customer, cacheOptions);


            return Ok(customer);
        }

        [HttpGet("phone/{phoneNumber}")]
        public async Task<IActionResult> GetCustomerByPhoneNumber(string phoneNumber)
        {
            try
            {
                var cacheKey = $"Customer_Phone_{phoneNumber}";
                if (_cache.TryGetValue(cacheKey, out string customerFullName))
                {
                    return Ok(customerFullName);
                }
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

                if (customer == null)
                {
                    return NotFound("Клиент с указанным номером телефона не найден.");
                }
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, customer.FullName, cacheOptions);
                return Ok(customer.FullName);
            }
            catch (Exception ex)
            {
                _serilogLogger.LogError(ex, "Произошла ошибка при получении клиента по номеру телефона");
                return StatusCode(500, $"Произошла ошибка при получении клиента по номеру телефона: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/orders/customer")]
        public async Task<IActionResult> GetOrdersForCustomer([FromQuery] string fullName, DateTime startDate, DateTime endDate)
        {
            try
            {

                var cacheKey = $"Orders_{fullName}_{startDate}_{endDate}";
                if (_cache.TryGetValue(cacheKey, out List<Orders> cachedOrders))
                {
                    return Ok(cachedOrders);
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
                    .Select(o => new { o.Order.OrderId, o.Order.OrderDate })
                    .ToListAsync();

                if (!orders.Any())
                {
                    return NotFound("Заказы для указанного клиента не найдены.");
                }

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                _cache.Set(cacheKey, orders, cacheOptions);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _serilogLogger.LogError(ex, "Произошла ошибка при получении заказов по ФИО клиента");
                return StatusCode(500, $"Произошла ошибка при получении заказов по ФИО клиента: {ex.Message}");
            }
        }
    }
}