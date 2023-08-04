using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Admin.Dashboard;

public class Index : ClientModel
{
    public decimal WeeklySales;
    public decimal TotalOrders;
    public decimal TotalCustomers;
    public decimal TotalGuest;
    public List<int> Years = new();

    public IActionResult OnGet()
    {
        if (!IsAdmin())
        {
            return ToForbiddenPage();
        }

        DashboardStatisticResponse statistic =
            CallGet<DashboardStatisticResponse>($"https://localhost:7176/Order/Statistic");

        WeeklySales = statistic.WeeklySales;
        TotalOrders = statistic.TotalOrders;
        TotalCustomers = statistic.TotalCustomers;
        TotalGuest = statistic.TotalGuest;
        Years = statistic.Years;
        return Page();
    }


    public IActionResult OnGetBigChart(int? year)
    {
        if (!IsAdmin())
        {
            return ToForbiddenPage();
        }
        
        List<int> orderCountOfEachMonth =
            CallGet<List<int>>($"https://localhost:7176/Order/OrderStatistic?year={year}");

        return new ObjectResult(orderCountOfEachMonth);
    }

    public IActionResult OnGetSmallChart()
    {
        if (!IsAdmin())
        {
            return ToForbiddenPage();
        }

        List<int> result = CallGet<List<int>>($"https://localhost:7176/Order/CustomerStatistic");

        return new ObjectResult(result);
    }
}