using API.Entities;

namespace API.Repositories
{
    public class ProductRepository: BaseRepository<Product, int>
    {
        protected override int GetId(Product entity)
        {
            return entity.ProductId;
        }

        public override void PreLoad()
        {
            DbContext.Categories.ToList();
        }
        
        public List<Product> FindTopSellers(int total)
        {
            HashSet<int> bestSalerIds = DbContext.OrderDetails
                .GroupBy(p => p.ProductId)
                .Select(p => new { ProductId = p.Key, Count = p.Count() })
                .OrderByDescending(x => x.Count)
                .Take(total).Select(x => x.ProductId)
                .ToHashSet();

            return DbContext.Products.Where(x => bestSalerIds.Contains(x.ProductId)).ToList();
        }
        
        public List<Product> FindTopNewest(int total)
        {
            return DbContext.Products.OrderByDescending(x => x.CreatedAt).Take(total).ToList();
        }

        public ProductRepository(PRN221_DBContext context) : base(context)
        {
        }
    }
}
