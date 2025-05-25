using Microsoft.AspNetCore.Mvc;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly int _pageSize = 8;

        public StoreController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index(
            int pageIndex,
            string? search,
            string? brand,
            string? gender,
            string? size,
            string? category,
            string? sort)
        {
            IQueryable<Product> query = _productRepository.GetAvailable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(p => p.Brand.Contains(brand));

            if (!string.IsNullOrWhiteSpace(gender))
                query = query.Where(p => p.Gender.ToLower() == gender.ToLower());

            if (!string.IsNullOrWhiteSpace(size))
                query = query.Where(p => p.Size.Contains(size));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.Contains(category));

            // Sorting
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderByDescending(p => p.Id)
            };

            // Pagination
            if (pageIndex < 1) pageIndex = 1;

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)_pageSize);

            var products = query
                .Skip((pageIndex - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            // Data to View
            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel
            {
                Search = search,
                Brand = brand,
                Gender = gender,
                Size = size,
                Category = category,
                Sort = sort
            };

            return View(storeSearchModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return RedirectToAction("Index");

            return View(product);
        }
    }
}
