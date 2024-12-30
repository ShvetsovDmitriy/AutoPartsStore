using AutoPartsStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartsStore.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string productName, [FromQuery] int? productTypeId, 
                                                     [FromQuery] bool? inStockOnly, [FromQuery] string sortBy)
        {
            try
            {
                var products = await _productService.GetProductsAsync(productName, productTypeId, inStockOnly, sortBy);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
