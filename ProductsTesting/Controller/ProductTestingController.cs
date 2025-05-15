using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.models;
using ProductAPI;
using Xunit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Identity.Client;

namespace ProductAPI.ProductsTesting.Controller
{
    public class ProductTestingController
    {

        private readonly Mock<ProductDbContext> _mockdb;


        //Creating inMemory Database to simulate a real database for testing
        //A new database for each test is created to make sure the tests does not affect each other
        //If the database is empty, it initialises it with three products.
        private ProductDbContext GetInMemoryDbContex()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ProductDbContext(options);    

            if(context.Products.CountAsync().Result == 0)
            {
                context.Products.AddRange(
                    new Product { Id = 4, ProductName = "Test product 1", Description = "This is test product 1", Price = 20 },
                    new Product { Id = 5, ProductName = "Test product 2", Description = "This is test product 2", Price = 30 },
                    new Product { Id = 6, ProductName = "Test product 3", Description = "This is test product 3", Price = 40 }
                    );
                context.SaveChanges();
            }


            return context;

        }



      

        //getting products
          //Checks if the GetProducts() method works as intended.
          //- Calls the GetProducts() method from the ProductController
          //-Asserts that the response is 200 OK
          //-Asserts that the response contains a list of products
          //-Ensures the list id not null
        [Fact]
        public async Task GetProducts_ReturnOkResult_WithListOfProduct()
        {
            using var context = GetInMemoryDbContex();
            var controller = new ProductController(context);

            var result = await controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.NotNull(products);
        }

        //getting product by Id
          //Checks if the GetProductById() method works as intended.
          //-Calls the GetProductById() method with the target id of 4
          //-Assert the returned object is of type Product
          //- Ensures the product id is 4
        [Fact]
        public async Task GetProductById_ReturnOkResult_WithProduct()
        {
            using var context = GetInMemoryDbContex();
       


            var controller = new ProductController(context);

            var result = await controller.GetProductById(4);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.NotNull(product);
            Assert.Equal(4, product.Id);
        }


        //Checks if returning a non-existent product id returns a 404 not Found 
          //-Calss the GetProductById() method with a non-existent target id of 60
          //- Assert the response is 404 Not Found
        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            using var context = GetInMemoryDbContex();
            var controller = new ProductController(context);

            var result = await controller.GetProductById(60);

            Assert.IsType<NotFoundResult>(result.Result);
        }


    }
}
