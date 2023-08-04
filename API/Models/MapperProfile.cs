using API.Entities;
using API.Models.Requests.Post;
using API.Models.Responses;
using AutoMapper;

namespace API.Models;

public class MapperProfile: Profile
{
    public MapperProfile()
    {
        CreateMap<NewProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
        CreateMap<Product, UpdateProductRequest>();
        CreateMap<Product, ProductResponse>();
        CreateMap<Product, ExportExcelProductResponse>();

        CreateMap<Category, CategoryResponse>();

        CreateMap<Account, AuthUser>();
        CreateMap<Account, AccountResponse>();

        CreateMap<Customer, CustomerResponse>();
        
        CreateMap<Order, OrderResponse>();
        CreateMap<Order, ExportExcelOrderResponse>();

        CreateMap<Employee, EmployeeResponse>();
        CreateMap<NewEmployeeRequest, Employee>();
        CreateMap<UpdateEmployeeRequest, Employee>();
        CreateMap<Employee, UpdateEmployeeRequest>();

        CreateMap<Department, DepartmentResponse>();

        CreateMap<OrderDetail, OrderDetailResponse>();
    }
}