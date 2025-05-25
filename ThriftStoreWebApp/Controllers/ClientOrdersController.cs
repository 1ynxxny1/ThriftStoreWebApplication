using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Service.DTOs;

namespace ThriftStoreWebApp.Controllers
{
    [Authorize(Roles = "client")]
    [Route("/Client/Orders/{action=Index}/{id?}")]
    public class ClientOrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private const int PageSize = 5;

        public ClientOrdersController(
            IOrderRepository orderRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageIndex)
        {
            if (pageIndex <= 0) pageIndex = 1;

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            var ordersQuery = _orderRepository
                .GetAll()
                .Where(o => o.ClientId == currentUser.Id)
                .OrderByDescending(o => o.Id);

            int totalCount = ordersQuery.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var pagedOrders = ordersQuery
                .Skip((pageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var orderDtos = _mapper.Map<List<OrderDto>>(pagedOrders);

            ViewBag.Orders = orderDtos;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View(orderDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            var order = await _orderRepository.GetDetailedByIdAsync(id);
            if (order == null || order.ClientId != currentUser.Id)
                return RedirectToAction("Index");

            var orderDto = _mapper.Map<OrderDto>(order);
            return View(orderDto);
        }
    }
}
