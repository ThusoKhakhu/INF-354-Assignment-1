using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.models;
using System.Runtime.InteropServices;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductController(ProductDbContext context) => _context = context;

        //create action method 
        [HttpGet]
        [Route("getProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products
                .OrderByDescending(p => p.Id) //Display the newly added product first
                .ToListAsync();
            return Ok(products);
        }

        // Creating action method to get a product by its ID
        [HttpGet]
        [Route("getProduct/{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(); 
            }

            return Ok(product);
        }


        //Adding a product
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Invalid product data.");
            }

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                // Return a structured JSON response
                return Ok(new { message = "Product added successfully", productName = product.ProductName });
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, new { message = "An error occurred while adding the product", error = ex.Message });
            }
        }

        //View model for updating
        public class UpdateModel
        {
            public string productName { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
        }

        [HttpPut]
        [Route("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateModel updateModel)
        {
            var productInDb = await _context.Products.FindAsync(id);
            if (productInDb == null)
            {
                return NotFound("Product not found.");
            }

            // Map the view model to the entity
            productInDb.ProductName = updateModel.productName;
            productInDb.Description = updateModel.Description;
            productInDb.Price = updateModel.Price;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product details updated." });
        }


        [HttpDelete]
        [Route("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Find the product by ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });  // Return a JSON response
            }

            // Remove the product
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Return a success message in JSON format
            return Ok(new { message = "Product deleted successfully" });
        }

        //Dependencies for Testing
        //dotnet add package xunit
        //dotnet add package xunit.runner.visualstudio
        //dotnet add package Moq
        //dotnet add package Microsoft.AspNetCore.Mvc.Testing
        //dotnet add package Newtonsoft.Json

        //Add project reference
        //dotnet add reference../YourProject.Api/YourProject.Api.csproj





    }
}
