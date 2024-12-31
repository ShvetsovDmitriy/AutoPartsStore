using AutoPartsStore.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Controllers.Auth
{
    public class AuthController(AccountService accountService) : Controller
    {
        [HttpGet("register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            accountService.Register(request.UserName, request.FirstName, request.Password);
            return NoContent();
        }
        [HttpPost("login")]
        public IActionResult login([FromBody] LoginRequest loginRequest)
        {
            var token = accountService.Login(loginRequest.UserName, loginRequest.Password);
            return Ok();
        }
    }
}
