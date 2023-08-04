using API.Entities;
using API.Models.Domain;
using API.Models.Exceptions;
using API.Models.Requests;
using API.Models.Requests.Post;
using API.Models.Responses;
using API.Repositories;
using Aspose.Pdf.Drawing;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly AccountRepository _accountRepository;
        private readonly OrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public EmployeeController(EmployeeRepository employeeRepository, IMapper mapper, OrderRepository orderRepository, AccountRepository accountRepository)
        {
            _employeeRepository = employeeRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [Route("")]
        public ListResult<EmployeeResponse> FindAll(String? name, int page = 1, int size = 10, bool isAscending = false, String orderBy = "EmployeeId")
        {
            ListResult<Employee> listResult = _employeeRepository.Search(page, size, isAscending, orderBy, whereClause: x => (string.IsNullOrWhiteSpace(name) || x.FirstName.ToLower().Contains(name.ToLower()) || x.LastName.ToLower().Contains(name.ToLower())));
            List<EmployeeResponse> responses = listResult.Content.Select(employee => _EmployeeResponseOf(employee)).ToList();
            return listResult.WithContent(responses);
        }

        [HttpGet]
        [Route("{id}")]
        public EmployeeResponse FindEmployee(int id)
        {
            Employee employee = _employeeRepository.GetOne(id, "Không tìm thấy nhân viên.");
            return _EmployeeResponseOf(employee);
        }

        [HttpPost]
        [Route("")]
        public EmployeeResponse AddPEmployee(NewEmployeeRequest request)
        {
            Employee employee = _mapper.Map<Employee>(request);
            Employee createdEmployee = _employeeRepository.Add(employee);

            return _EmployeeResponseOf(createdEmployee);
        }

        [HttpPut]
        [Route("{id}")]
        public EmployeeResponse UpdateEmployee(int id, UpdateEmployeeRequest request)
        {
            Employee employee = _employeeRepository.GetOne(id, "Không tìm thấy nhân viên.");
            if (request.LastName != null)
            {
                employee.LastName = request.LastName;
            }
            if (request.FirstName != null)
            {
                employee.FirstName = request.FirstName;
            }
            if (request.Title != null)
            {
                employee.Title = request.Title;
            }
            if (request.TitleOfCourtesy != null)
            {
                employee.TitleOfCourtesy = request.TitleOfCourtesy;
            }
            if (request.DepartmentId != null)
            {
                employee.DepartmentId = request.DepartmentId;
            }
            if (request.BirthDate != null)
            {
                employee.BirthDate = request.BirthDate;
            }
            if (request.HireDate != null)
            {
                employee.HireDate = request.HireDate;
            }
            if (request.Address != null)
            {
                employee.Address = request.Address;
            }
            Employee updateEmployee = _employeeRepository.Update(employee);

            return _EmployeeResponseOf(updateEmployee);
        }

        [HttpDelete]
        [Route("{id}")]
        public void DeleteEmployee(int id)
        {
            Employee employee = _employeeRepository.GetOne(id, "Không tìm thấy sản phẩm.");
            //Account currentAccount = _accountRepository.GetOne(accountId);
            //Employee empInAcc = _employeeRepository.FindOne(x => x.EmployeeId != null && x.EmployeeId == currentAccount.EmployeeId);
            List<Order> empInOrder = _orderRepository.FindAll(x => x.EmployeeId == id).ToList();
            //if (empInAcc != null)
            //{               
            //    if(empInAcc.EmployeeId == id)
            //    {
            //        throw ApiException.BadRequest("Cant delete yourself");
            //    }
            //}
            if (empInOrder.Count != 0)
            {
                throw ApiException.BadRequest("Cant delete employee because he/she has order");
            }
            _employeeRepository.Delete(employee);
            //return "Đã xóa nhân viên thành công.";
        }

        private EmployeeResponse _EmployeeResponseOf(Employee employee)
        {
            EmployeeResponse response = _mapper.Map<EmployeeResponse>(employee);
            response.Department = _mapper.Map<DepartmentResponse>(employee.Department);
            return response;
        }
    }
}
