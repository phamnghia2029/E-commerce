using API.Entities;

namespace API.Repositories
{
    public class CustomerRepository: BaseRepository<Customer, string>
    {
        protected override string GetId(Customer entity)
        {
            return entity.CustomerId;
        }

        public override void PreLoad()
        {
        }

        public CustomerRepository(PRN221_DBContext context) : base(context)
        {
        }
    }
}
