using Microsoft.EntityFrameworkCore;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Services;

namespace ThriftStoreWebApp.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAll() => _context.Products;

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetLatestAvailableAsync(int count)
        {
            return await _context.Products
                .Where(p => p.Availability)
                .OrderByDescending(p => p.Id)
                .Take(count)
                .ToListAsync();
        }

        public IQueryable<Product> GetAvailable()
        {
            return _context.Products.Where(p => p.Availability);
        }

    }
}
