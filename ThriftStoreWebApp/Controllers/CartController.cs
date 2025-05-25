using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;

namespace ThriftStoreWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly decimal _shippingFee;

        public CartController(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
        }

        public IActionResult Index()
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, _productRepository);
            var subtotal = CartHelper.GetSubtotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = _shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + _shippingFee;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutDto model)
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, _productRepository);
            var subtotal = CartHelper.GetSubtotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = _shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + _shippingFee;

            if (!ModelState.IsValid)
                return View(model);

            if (!cartItems.Any())
            {
                ViewBag.ErrorMessage = "Your cart is empty";
                return View(model);
            }

            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, _productRepository);
            var total = CartHelper.GetSubtotal(cartItems) + _shippingFee;
            var cartSize = cartItems.Sum(item => item.Quantity);

            var deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            var paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if (cartSize == 0 || string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(paymentMethod))
                return RedirectToAction("Index", "Home");

            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Total = total;
            ViewBag.CartSize = cartSize;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(int any)
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, _productRepository);

            var deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            var paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if (!cartItems.Any() || string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(paymentMethod))
                return RedirectToAction("Index", "Home");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var unavailableItems = new List<string>();

            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.Product.Id);
                if (product == null || !product.Availability)
                    unavailableItems.Add(item.Product.Name);
            }

            if (unavailableItems.Any())
            {
                TempData["ErrorMessage"] = "The following items are no longer available: " + string.Join(", ", unavailableItems);
                return RedirectToAction("Index");
            }

            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.Product.Id);
                product.Availability = false;
                await _productRepository.UpdateAsync(product);
            }

            var order = new Order
            {
                ClientId = user.Id,
                Items = cartItems,
                ShippingFee = _shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "pending",
                PaymentDetails = "",
                OrderStatus = "created",
                CreatedAt = DateTime.Now
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            Response.Cookies.Delete("shopping_cart");

            ViewBag.SuccessMessage = "Order created successfully!";
            return View();
        }
    }
}
