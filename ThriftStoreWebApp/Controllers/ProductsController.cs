using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly int pageSize = 5;

        public ProductsController(IProductRepository productRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _productRepository = productRepository;
            _environment = environment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, string? search = null, string? column = null, string? orderBy = null)
        {
            IQueryable<Product> query = _productRepository.GetAll();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            string[] validColumns = { "Id", "Name", "Brand", "Gender", "Size", "Category", "Price", "CreatedDate", "Availability" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column)) column = "Id";
            if (!validOrderBy.Contains(orderBy)) orderBy = "desc";

            query = column switch
            {
                "Name" => orderBy == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "Brand" => orderBy == "asc" ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                "Gender" => orderBy == "asc" ? query.OrderBy(p => p.Gender) : query.OrderByDescending(p => p.Gender),
                "Size" => orderBy == "asc" ? query.OrderBy(p => p.Size) : query.OrderByDescending(p => p.Size),
                "Category" => orderBy == "asc" ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
                "Price" => orderBy == "asc" ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "CreatedDate" => orderBy == "asc" ? query.OrderBy(p => p.CreatedDate) : query.OrderByDescending(p => p.CreatedDate),
                "Availability" => orderBy == "asc" ? query.OrderBy(p => p.Availability) : query.OrderByDescending(p => p.Availability),
                _ => orderBy == "asc" ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id)
            };

            int totalPages = (int)Math.Ceiling(query.Count() / (double)pageSize);
            var products = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            ViewData["Search"] = search ?? "";
            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFile!.FileName);
            string imageFullPath = Path.Combine(_environment.WebRootPath, "products images", newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            var product = _mapper.Map<Product>(productDto);
            product.ImageFileName = newFileName;
            product.CreatedDate = DateTime.Now;

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");

            var productDto = _mapper.Map<ProductDto>(product);

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedDate"] = product.CreatedDate.ToString("dd/MM/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedDate"] = product.CreatedDate.ToString("dd/MM/yyyy");
                return View(productDto);
            }

            string newFileName = product.ImageFileName ?? string.Empty;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFile.FileName);
                string imageFullPath = Path.Combine(_environment.WebRootPath, "products images", newFileName);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                string oldImageFullPath = Path.Combine(_environment.WebRootPath, "products images", product.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath)) System.IO.File.Delete(oldImageFullPath);
            }

            _mapper.Map(productDto, product); 
            product.ImageFileName = newFileName;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");

            string imageFullPath = Path.Combine(_environment.WebRootPath, "products images", product.ImageFileName);
            if (System.IO.File.Exists(imageFullPath)) System.IO.File.Delete(imageFullPath);

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
