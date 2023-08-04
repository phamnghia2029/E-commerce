using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class CanceledOrders : ClientModel
{
    public AccountResponse Account;
    public List<OrderResponse> Orders { get; set; }
    public IActionResult OnGet()
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }

        Account = GetCurrentUser();
        Orders = CallGet<List<OrderResponse>>($"https://localhost:7176/Order?customerId={Account.CustomerId}&canceled=true").OrderByDescending(x => x.OrderDate).ToList();
        return Page();
    }
}