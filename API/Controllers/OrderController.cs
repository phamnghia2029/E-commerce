using API.Entities;
using API.Models;
using API.Models.Domain;
using API.Models.Requests;
using API.Models.Responses;
using API.Repositories;
using API.Utils;
using Aspose.Pdf.Operators;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController
{
    private readonly ProductRepository _productRepository;
    private readonly AccountRepository _accountRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly OrderRepository _orderRepository;
    private readonly OrderDetailRepository _orderDetailRepository;
    private readonly IMapper _mapper;

    public OrderController(OrderRepository orderRepository, IMapper mapper, OrderDetailRepository orderDetailRepository, CustomerRepository customerRepository, AccountRepository accountRepository, ProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _orderDetailRepository = orderDetailRepository;
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
        _productRepository = productRepository;
    }
        
    [HttpGet]
    [Route("")]
    public List<OrderResponse> GetAll(string? customerId, bool? canceled)
    {
        return _orderRepository.FindAll(
            x => (customerId == null || customerId.Equals(x.CustomerId)) || (canceled == null || (x.RequiredDate == null) == canceled)
        ).Select(order => ResponseOf(order)).ToList();
    }

    [HttpGet]
    [Route("byDate")]
    public List<ExportExcelOrderResponse> GetAllToExport(DateTime? startDate, DateTime? endDate)
    {
        return _orderRepository.FindAll(
            x => (startDate == null || startDate < x.OrderDate) && (endDate == null || x.OrderDate < endDate)
        ).Select(order => ExportExcelOrderResponseOf(order)).ToList();
    }

    [HttpGet]
    [Route("AdminOrder")]
    public ListResult<OrderResponse> GetAllByAdmin(DateTime? startDate, DateTime? endDate, int page = 1, int size = 10, bool isAscending = false, String orderBy = "OrderId")
    {
        ListResult<Order> listResult = _orderRepository.Search(page, size, isAscending, orderBy, whereClause: x => (startDate == null || startDate < x.OrderDate) && (endDate == null || x.OrderDate < endDate));
        List<OrderResponse> responses = listResult.Content.Select(order => ResponseOf(order)).ToList();
        return listResult.WithContent(responses);
    }

    [HttpGet]
    [Route("detail/{id}")]
    public List<OrderResponse> GetAllById(int id)
    {
        return _orderRepository.FindAll(x => x.OrderId == id).Select(order => ResponseOf(order)).ToList();
    }

    private OrderResponse ResponseOf(Order order)
    {
        //OrderResponse response = _mapper.Map<OrderResponse>(order);
        //return response;
        return _mapper.Map<OrderResponse>(order);
    }

    private ExportExcelOrderResponse ExportExcelOrderResponseOf(Order order)
    {
        //OrderResponse response = _mapper.Map<OrderResponse>(order);
        //return response;
        return _mapper.Map<ExportExcelOrderResponse>(order);
    }

    [HttpGet]
    [Route("{id}")]
    public OrderResponse GetOne(int id)
    {
        return ResponseOf(_orderRepository.GetOne(id, "Order not found"));
    }
        
    [HttpPatch]
    [Route("{id}/Cancel")]
    public void CancelOrder(int id)
    {
        Order order = _orderRepository.GetOne(id, "Order not found");
        order.RequiredDate = null;
        _orderRepository.Update(order);
    }

    [HttpPatch]
    [Route("{id}/Confirm")]
    public void ConfirmOrder(int id)
    {
        Order order = _orderRepository.GetOne(id, "Order not found");
        order.ShippedDate = DateTime.Now;
        _orderRepository.Update(order);
    }

    [HttpPost]
    [Route("Purchase")]
    public void PurchaseCart(PurchaseRequest request)
    {
        Order orderToAdd = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.Now,
            Freight = request.Cart.GetAll().Sum(p => p.Product.UnitPrice * p.count),
            RequiredDate = request.OrderDate
        };
        _orderRepository.Add(orderToAdd);
        
        var orderDetails = SaveOrderDetails(request, orderToAdd);

        PurchaseCartEmailCommand command = new PurchaseCartEmailCommand
        {
            OrderDetails = orderDetails,
            Customer = _customerRepository.GetOne(request.CustomerId),
            email = request.Email,
            Order = orderToAdd
        };
        command.Execute();
    }

    private List<OrderDetail> SaveOrderDetails(PurchaseRequest request, Order orderToAdd)
    {
        _orderDetailRepository.PreLoad();
        List<OrderDetail> orderDetails = request.Cart.GetAll().Select(p =>
        {
            OrderDetail orderDetail = new OrderDetail
            {
                OrderId = orderToAdd.OrderId,
                ProductId = p.Product.ProductId,
                UnitPrice = (decimal)(p.Product.UnitPrice * p.count),
                Quantity = (short)p.count
            };
            return orderDetail;
        }).ToList();
        return _orderDetailRepository.AddAll(orderDetails);
    }
    
            
    [HttpGet]
    [Route("Statistic")]
    public DashboardStatisticResponse Statistic()
    {
        List<Order> orders = _orderRepository.FindAll();

        decimal totalCustomers = _customerRepository.FindAll().Select(x => x.CustomerId).Where(x => x != null).Distinct().Count();
        
        List<int> years = orders.Where(x => x.OrderDate != null).Select(x => x.OrderDate.Value.Year).Distinct().OrderBy(x => x).ToList();
        
        HashSet<string?> nonGuestCustomerIds = _accountRepository.FindAll().Select(x => x.CustomerId).Where(x => x != null).ToHashSet();
        decimal totalGuest = orders
            .Select(x => x.Customer.CustomerId)
            .Count(customerId => !nonGuestCustomerIds.Contains(customerId));
        
        DateTime startOfThisWeek = Moments.StartDayOf(Moments.DayOfThisWeekOn(DayOfWeek.Monday));
        decimal weeklySales = orders
            .Where(order => order.OrderDate > startOfThisWeek && order.OrderDate < DateTime.Now)
            .SelectMany(order => order.OrderDetails)
            .Sum(orderDetail => orderDetail.Quantity * orderDetail.UnitPrice);

        return new DashboardStatisticResponse
        {
            TotalCustomers = totalCustomers,
            TotalOrders = orders.Count(),
            TotalGuest = totalGuest,
            WeeklySales = weeklySales,
            Years = years,
        };
    }
    
            
    [HttpGet]
    [Route("OrderStatistic")]
    public List<int> OrderCountOfEachMonthOfYear(int? year)
    {
        List<int> orderCountOfEachMonth = Collections.ListOf<int>(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        List<int> yearsInOrderDate = _orderRepository.YearsInOrderDate();

        if (year == null) year = yearsInOrderDate[0];
        List<Order> orders = _orderRepository.FindAll(o => o.OrderDate.Value.Year == year);

        orders.ForEach(order =>
        {
            int month = order.OrderDate.Value.Month;
            orderCountOfEachMonth[month - 1] += 1;
        });
        return orderCountOfEachMonth;
    }
    
            
    [HttpGet]
    [Route("CustomerStatistic")]
    public List<int> CustomerStatistic(int year = 1996)
    {
        DateTime firstDayOfLast30Days = DateTime.Now.AddDays(-30);

        List<Customer> customers = _customerRepository.FindAll();
        List<Customer> newCustomers = customers.Where(c => c.CreatedAt > firstDayOfLast30Days).ToList();

        int totalZero = customers.Count.ToString().Count() - 1;
        int roundNumber = (int) Math.Pow(10, totalZero);

        return Collections.ListOf(
            customers.Count, 
            newCustomers.Count,
            customers.Count / roundNumber * roundNumber + roundNumber
        );
    }
}