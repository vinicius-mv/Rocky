using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System.Linq;

namespace Rocky.DataAccess.Repository.Interfaces
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            var orderDetailFromDb = _context.OrderDetails.AsNoTracking().FirstOrDefault(x => x.Id == orderDetail.Id);

            if (orderDetailFromDb != null)
            {
                // Update all the properties
                _context.OrderDetails.Update(orderDetail);
            }
        }
    }
}