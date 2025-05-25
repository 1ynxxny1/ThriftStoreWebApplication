using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThriftStoreWebApp.Data.Repositories;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;
using Xunit;

namespace ThriftStoreWebApp.Tests.Services
{
    public class ProductTest
    {
        private async Task<ProductRepository> GetRepositoryWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Products.Add(new Product { Name = "T-Shirt", Brand = "Nike", Price = 29.99m, Availability = true });
            context.Products.Add(new Product { Name = "Jeans", Brand = "Levi's", Price = 59.99m, Availability = true });
            await context.SaveChangesAsync();

            return new ProductRepository(context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            var repository = await GetRepositoryWithData();
            var products = await repository.GetAllAsync();
            Assert.Equal(2, products.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectProduct()
        {
            var repository = await GetRepositoryWithData();
            var all = await repository.GetAllAsync();
            var first = all.First();

            var product = await repository.GetByIdAsync(first.Id);

            Assert.NotNull(product);
            Assert.Equal(first.Name, product!.Name);
        }

        [Fact]
        public async Task AddAsync_AddsProductSuccessfully()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var repository = new ProductRepository(context);

            var product = new Product
            {
                Name = "Jacket",
                Brand = "Adidas",
                Price = 79.99m,
                Availability = true
            };

            await repository.AddAsync(product);
            await repository.SaveChangesAsync();

            Assert.Equal(1, context.Products.Count());
            Assert.Equal("Jacket", context.Products.First().Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesProductSuccessfully()
        {
            var repository = await GetRepositoryWithData();
            var product = (await repository.GetAllAsync()).First();
            product.Name = "Updated Name";

            await repository.UpdateAsync(product);
            await repository.SaveChangesAsync();

            var updated = await repository.GetByIdAsync(product.Id);
            Assert.Equal("Updated Name", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesProductSuccessfully()
        {
            var repository = await GetRepositoryWithData();
            var product = (await repository.GetAllAsync()).First();

            await repository.DeleteAsync(product);
            await repository.SaveChangesAsync();

            var all = await repository.GetAllAsync();
            Assert.Single(all); 
        }
    }
}
