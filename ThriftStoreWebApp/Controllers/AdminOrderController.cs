using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;

namespace ThriftStoreWebApp.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/AdminOrders/{action=Index}/{id?}")]

    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 5;

        public AdminOrderController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex)
        {
            IQueryable<Order> query = context.Orders.Include(o => o.Client)
                .Include(o => o.Items).OrderByDescending(o => o.Id);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var orders = query.ToList();

            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        public IActionResult Details(int id)
        {
            // Fetch the order including related client and items
            var order = context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);

            // Check if the order is null
            if (order == null)
            {
                // Log the issue (if you have a logger)
                // logger.LogWarning($"Order with ID {id} not found.");

                // Redirect to the index page or show a not found view
                return RedirectToAction("Index"); // or return View("NotFound");
            }

            // Count the number of orders for the client
            ViewBag.NumOrders = context.Orders.Count(o => o.ClientId == order.ClientId);

            // Return the order details view
            return View(order);
        }

        public IActionResult Edit(int id, string? payment_status, string? order_status)
        {
            var order = context.Orders.Find(id);
            if (order == null )
            {
                return RedirectToAction("Index");
            }

            if (payment_status == null && order_status == null)
            {
                return RedirectToAction("Details", new { id });
            }

            if (payment_status != null)
            {
                order.PaymentStatus = payment_status;
            }

            if (order_status != null)
            {
                order.OrderStatus = order_status;
            }

            context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

    }
}
