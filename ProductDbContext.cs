using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProductAPI.models;

namespace ProductAPI
{
    public class ProductDbContext: DbContext
    {
        //create constructor of Product DB 
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        { 
                    
        }
        public DbSet<Product> Products { get; set; }

        //*****can add method to seed initial data ****/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, ProductName = "Product 1", Price = 10, Description = "This is product 1" },
                new Product { Id = 2, ProductName = "Product 2", Price = 20, Description = "This is product 2" },
                new Product { Id = 3, ProductName = "Product 3", Price = 30 , Description = "This is product 3" }
            );
        }
    }
}
