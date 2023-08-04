namespace API.Models.Responses;

public class DashboardStatisticResponse
{
    public decimal WeeklySales { get; set; }
    public decimal TotalOrders { get; set; }
    public decimal TotalCustomers { get; set; }
    public decimal TotalGuest { get; set; }
    public List<int> Years { get; set; }
}