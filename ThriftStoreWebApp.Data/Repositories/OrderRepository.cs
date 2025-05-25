using Microsoft.EntityFrameworkCore;
using ThriftStoreWebApp.Data.Interfaces;
using ThriftStoreWebApp.Models;
using ThriftStoreWebApp.Services;

namespace ThriftStoreWebApp.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Order> GetAll()
        {
            return context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items);
        }

        public async Task<Order?> GetDetailedByIdAsync(int id)
        {
            return await context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await context.Orders.FindAsync(id);
        }

        public async Task AddAsync(Order order)
        {
            await context.Orders.AddAsync(order);
        }

        public Task UpdateAsync(Order order)
        {
            context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Order order)
        {
            context.Orders.Remove(order);
            return Task.CompletedTask;
        }


        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
