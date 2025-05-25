using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Service.DTOs;

namespace ThriftStoreWebApp.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/AdminOrders/{action=Index}/{id?}")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly int _pageSize = 5;

        public AdminOrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            if (pageIndex < 1)
                pageIndex = 1;

            var ordersQuery = _orderRepository.GetAll().OrderByDescending(o => o.Id);
            int totalCount = ordersQuery.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)_pageSize);

            var pagedOrders = ordersQuery
                .Skip((pageIndex - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            var orderDtos = _mapper.Map<List<OrderDto>>(pagedOrders);

            ViewBag.Orders = orderDtos;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View(orderDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetDetailedByIdAsync(id);
            if (order == null)
                return RedirectToAction("Index");

            var orderDto = _mapper.Map<OrderDto>(order);
            ViewBag.NumOrders = _orderRepository.GetAll().Count(o => o.ClientId == order.ClientId);

            return View(orderDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? payment_status, string? order_status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return RedirectToAction("Index");

            if (!string.IsNullOrWhiteSpace(payment_status))
                order.PaymentStatus = payment_status;

            if (!string.IsNullOrWhiteSpace(order_status))
                order.OrderStatus = order_status;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }
    }
}
