using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System.Linq;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            var orderHeaderFromDb = _context.OrderHeaders.AsNoTracking().FirstOrDefault(x => x.Id == orderHeader.Id);

            if (orderHeaderFromDb != null)
            {
                // Update all the properties
                _context.OrderHeaders.Update(orderHeader);
            }
        }
    }
}