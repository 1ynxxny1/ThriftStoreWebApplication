using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Data.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        IQueryable<Order> GetAll(); 
        Task<Order?> GetDetailedByIdAsync(int id); 
        Task<Order?> GetByIdAsync(int id); 
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task SaveChangesAsync();

    }
}
