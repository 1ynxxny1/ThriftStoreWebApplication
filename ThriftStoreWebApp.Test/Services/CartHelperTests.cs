using System.Collections.Generic;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;
using Xunit;

namespace ThriftStoreWebApp.Tests.Services
{
    public class CartHelperTests
    {
        [Fact]
        public void GetSubtotal_ReturnsCorrectTotal()
        {
            // Arrange
            var items = new List<OrderItem>
            {
                new OrderItem { Quantity = 2, UnitPrice = 10 },
                new OrderItem { Quantity = 3, UnitPrice = 20 }
            };

            // Act
            var subtotal = CartHelper.GetSubtotal(items);

            // Assert
            Assert.Equal(2 * 10 + 3 * 20, subtotal);
        }
    }
}
