using API.Entities;
using API.Models.Domain;
using API.Models.Requests;
using API.Models.Responses;
using API.Repositories;
using API.Utils;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController
{
    private readonly CustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerController(CustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("")]
    public ListResult<CustomerResponse> FindAll(String? ContactName, int page = 1, int size = 10, bool isAscending = false, String orderBy = "CreatedAt")
    {
        ListResult<Customer> listResult = _customerRepository.Search(page, size, isAscending, orderBy, whereClause: x => (string.IsNullOrWhiteSpace(ContactName) || x.ContactName.ToLower().Contains(ContactName.ToLower())));
        List<CustomerResponse> responses = listResult.Content.Select(customer => _CustomerResponseOf(customer)).ToList();
        return listResult.WithContent(responses);
    }

    [HttpPut]
    [Route("{id}/SetStatus")]
    public CustomerResponse SetActiveAndDeactive(String? id)
    {
        Customer customer = _customerRepository.GetOne(id);
        if(customer.IsActive == true) customer.IsActive = false;
        else customer.IsActive = true;
        Customer updateCustomer = _customerRepository.Update(customer);
        return _CustomerResponseOf(updateCustomer);
    }

    [HttpPost]
    [Route("Guest")]
    public CustomerResponse CreateGuest(NewGuestCustomerRequest request)
    {
        var guestCustomer = new Customer
        {
            CustomerId = Strings.GenerateRandomStringWithLength(5),
            CompanyName = request?.CompanyName,
            ContactName = request?.ContactName,
            ContactTitle = request?.ContactTitle,
            Address = request?.Address,
            CreatedAt = DateTime.Now
        };
        var customer = _customerRepository.Add(guestCustomer);
        return _mapper.Map<CustomerResponse>(customer);
    }

    private CustomerResponse _CustomerResponseOf(Customer customer)
    {
        CustomerResponse response = _mapper.Map<CustomerResponse>(customer);
        return response;
    }
}