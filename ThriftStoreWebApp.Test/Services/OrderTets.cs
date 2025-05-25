using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThriftStoreWebApp.Data.Repositories;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;
using Xunit;

namespace ThriftStoreWebApp.Tests.Services
{
    public class OrderTest
    {
        private async Task<ApplicationDbContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();

            // Dummy clients
            var client1 = new ApplicationUser
            {
                Id = "client1",
                UserName = "client1@test.com",
                Email = "client1@test.com",
                CreatedAt = DateTime.Now
            };

            var client2 = new ApplicationUser
            {
                Id = "client2",
                UserName = "client2@test.com",
                Email = "client2@test.com",
                CreatedAt = DateTime.Now
            };

            context.Users.AddRange(client1, client2);

            // Dummy orders
            var order1 = new Order
            {
                Id = 1,
                ClientId = "client1",
                Client = client1,
                ShippingFee = 100,
                DeliveryAddress = "123 Street",
                PaymentMethod = "Cash",
                PaymentStatus = "pending",
                OrderStatus = "created",
                CreatedAt = DateTime.Now,
                Items = new List<OrderItem>()
            };

            var order2 = new Order
            {
                Id = 2,
                ClientId = "client2",
                Client = client2,
                ShippingFee = 50,
                DeliveryAddress = "456 Street",
                PaymentMethod = "Card",
                PaymentStatus = "accepted",
                OrderStatus = "shipped",
                CreatedAt = DateTime.Now,
                Items = new List<OrderItem>()
            };

            context.Orders.AddRange(order1, order2);

            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task GetAll_ReturnsAllOrders()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repo = new OrderRepository(context);

            // Act
            var result = repo.GetAll().ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectOrder()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repo = new OrderRepository(context);

            // Act
            var order = await repo.GetByIdAsync(1);

            // Assert
            Assert.NotNull(order);
            Assert.Equal("client1", order.ClientId);
        }

        [Fact]
        public async Task AddAsync_AddsOrderSuccessfully()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repo = new OrderRepository(context);

            var newOrder = new Order
            {
                ClientId = "client3",
                ShippingFee = 75,
                DeliveryAddress = "789 Street",
                PaymentMethod = "Cash",
                PaymentStatus = "pending",
                OrderStatus = "created",
                CreatedAt = DateTime.Now
            };

            // Act
            await repo.AddAsync(newOrder);
            await repo.SaveChangesAsync();

            // Assert
            Assert.Equal(3, context.Orders.Count());
        }

        [Fact]
        public async Task UpdateAsync_UpdatesOrderSuccessfully()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repo = new OrderRepository(context);

            var order = await repo.GetByIdAsync(1);
            order.PaymentStatus = "accepted";

            // Act
            await repo.UpdateAsync(order);
            await repo.SaveChangesAsync();

            // Assert
            var updatedOrder = await repo.GetByIdAsync(1);
            Assert.Equal("accepted", updatedOrder.PaymentStatus);
        }

        [Fact]
        public async Task DeleteAsync_RemovesOrderSuccessfully()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repo = new OrderRepository(context);

            var order = await repo.GetByIdAsync(1);

            // Act
            await repo.DeleteAsync(order);
            await repo.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Orders.Count());
        }
    }
}
