﻿
using AutoPartsStore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AutoPartsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerInfoController : ControllerBase
    {
        private readonly ICustomerInfoService _customerInfoService;

        public CustomerInfoController(ICustomerInfoService customerInfoService)
        {
            _customerInfoService = customerInfoService ?? throw new ArgumentNullException(nameof(customerInfoService));
        }

        [HttpGet("{CustomerId}")]
        public Task<IActionResult> GetCustomerById(int customerId)
        {
            return _customerInfoService.GetCustomerByIdAsync(customerId);
        }

        [HttpGet("phone/{phoneNumber}")]
        public Task<IActionResult> GetCustomerByPhoneNumber(string phoneNumber)
        {
            return _customerInfoService.GetCustomerByPhoneNumberAsync(phoneNumber);
        }
    }
}
