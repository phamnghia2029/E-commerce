using API.Entities;

namespace API.Repositories
{
    public class EmployeeRepository: BaseRepository<Employee, int>
    {
        protected override int GetId(Employee entity)
        {
            return entity.EmployeeId;
        }

        public override void PreLoad()
        {
            DbContext.Departments.ToList();
        }

        public EmployeeRepository(PRN221_DBContext context) : base(context)
        {
        }
    }
}
