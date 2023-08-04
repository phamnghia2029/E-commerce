using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class OrderRepository: BaseRepository<Order, int>
    {
        protected override int GetId(Order entity)
        {
            return entity.OrderId;
        }

        public override void PreLoad()
        {
            DbContext.OrderDetails.ToList();
            DbContext.Customers.ToList();
            DbContext.Products.ToList();
            DbContext.Employees.ToList();
        }

        public OrderRepository(PRN221_DBContext context) : base(context)
        {           
        }

        public List<int> YearsInOrderDate()
        {
            var orderDate = this.DbContext.Orders.Select(x => x.OrderDate).Distinct().ToList();
            List<int> yearsInOrderDate = orderDate.Where(x => x.HasValue).Select(x => (Convert.ToDateTime(x.Value).Year)).Distinct().ToList();
            return yearsInOrderDate;
        }
    }
}
