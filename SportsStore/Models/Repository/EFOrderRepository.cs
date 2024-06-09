using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models.Repository
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly StoreDbContext context;

        public EFOrderRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Order> Orders => this.context.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Product);

        public void SaveOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            this.context.AttachRange(order.Lines.Select(l => l.Product));

            if (order.OrderId == 0)
            {
                this.context.Orders.Add(order);
            }

            this.context.SaveChanges();
        }
    }
}
