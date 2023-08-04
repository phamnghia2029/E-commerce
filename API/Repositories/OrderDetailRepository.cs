using API.Entities;
using API.Models.Exceptions;

namespace API.Repositories
{
    public class OrderDetailRepository: BaseRepository<OrderDetail, int>
    {
        protected override int GetId(OrderDetail entity)
        {
            throw new ApiException();
        }

        public override void PreLoad()
        {
            DbContext.Products.ToList();
            DbContext.Orders.ToList();
        }

        public OrderDetailRepository(PRN221_DBContext context) : base(context)
        {
        }
    }
}
