using API.Entities;

namespace API.Repositories
{
    public class DepartmentRepository: BaseRepository<Department, int>
    {
        protected override int GetId(Department entity)
        {
            return entity.DepartmentId;
        }

        public override void PreLoad()
        {
        }

        public DepartmentRepository(PRN221_DBContext context) : base(context)
        {
        }
    }
}
