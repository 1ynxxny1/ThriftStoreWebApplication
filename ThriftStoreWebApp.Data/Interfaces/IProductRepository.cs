using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Data.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAll(); 
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
        Task<List<Product>> GetLatestAvailableAsync(int count);
        IQueryable<Product> GetAvailable(); 
    }
}
