using System.Text.Json;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Data.Interfaces;

namespace ThriftStoreWebApp.Services
{
    public static class CartHelper
    {
        public static Dictionary<int, int> GetCartDictionary(HttpRequest request, HttpResponse response)
        {
            string cookieValue = request.Cookies["shopping_cart"] ?? "";

            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(decoded);

                if (dictionary != null)
                {
                    return dictionary;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CartHelper] Failed to parse shopping_cart cookie: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(cookieValue))
            {
                response.Cookies.Delete("shopping_cart");
            }

            return new Dictionary<int, int>();
        }

        public static int GetCartSize(HttpRequest request, HttpResponse response)
        {
            return GetCartDictionary(request, response).Values.Sum();
        }

        public static List<OrderItem> GetCartItems(HttpRequest request, HttpResponse response, IProductRepository productRepository)
        {
            var cartItems = new List<OrderItem>();
            var cartDictionary = GetCartDictionary(request, response);

            foreach (var (productId, quantity) in cartDictionary)
            {
                var productTask = productRepository.GetByIdAsync(productId);
                productTask.Wait(); 
                var product = productTask.Result;

                if (product == null) continue;

                cartItems.Add(new OrderItem
                {
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    Product = product
                });
            }

            return cartItems;
        }

        public static decimal GetSubtotal(List<OrderItem> cartItems)
        {
            return cartItems.Sum(item => item.Quantity * item.UnitPrice);
        }
    }
}
