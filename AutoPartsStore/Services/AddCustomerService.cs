using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoPartsStore.Model;

namespace AutoPartsStore.Services
{
    public interface ICustomerAddService
    {
        Task<IActionResult> AddCustomerAsync(Customer customer);
    }
    public class CustomerAddService : ControllerBase,ICustomerAddService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<CustomerAddService> _logger;

        public CustomerAddService(MyDbContext context, ILogger<CustomerAddService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> AddCustomerAsync(Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                bool isRussianFullName = IsRussianFullName(customer.FullName);
                bool isValidPhoneNumber = IsValidPhoneNumber(customer.PhoneNumber);

                if (!isRussianFullName && !isValidPhoneNumber)
                {
                    return new BadRequestObjectResult(new
                    {
                        FullNameError = "ФИО должно быть только на русском языке.",
                        PhoneNumberError = "Номер телефона должен быть в 10-тизначном формате."
                    });
                }
                else if (!isRussianFullName)
                {
                    return new BadRequestObjectResult("ФИО должно быть только на русском языке.");
                }
                else if (!isValidPhoneNumber)
                {
                    return new BadRequestObjectResult("Номер телефона должен быть в 10-тизначном формате.");
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return new CreatedAtActionResult(nameof(AddCustomerAsync), null, new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при добавлении клиента");
                return new StatusCodeResult(500);
            }
        }

        private bool IsRussianFullName(string? fullName)
        {
            if (fullName == null)
            {
                return false;
            }

            return Regex.IsMatch(fullName, @"^[А-Яа-яЁё\s]+$");
        }

        private bool IsValidPhoneNumber(string? phoneNumber)
        {
            return phoneNumber != null && phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit);
        }
    }
}
