using API.Entities;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController
    {
        private readonly DepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentController(DepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public List<Department> GetAll()
        {
            return _departmentRepository.FindAll();
        }
    }
}
