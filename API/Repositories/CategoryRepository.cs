using API.Entities;

namespace API.Repositories
{
    public class CategoryRepository: BaseRepository<Category, int>
    {
        protected override int GetId(Category entity)
        {
            return entity.CategoryId;
        }

        public override void PreLoad()
        {
        }

        public CategoryRepository(PRN221_DBContext context) : base(context)
        {
        }
    }
}
