using API.Models.Domain;
using API.Models.Requests;
using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class Carting : ClientModel
{
    public Cart Cart { get; set; } = new();

    [BindProperty] 
    public CustomerResponse Customer { get; set; }

    public void OnGet()
    {
        Cart = GetCart();
        AccountResponse? account = GetCurrentUser();
        Customer = account?.Customer ?? new CustomerResponse();
        StoreTempData(account);
    }

    public IActionResult OnPostIncrease(int productId, int total)
    {
        OnGet();
        GetAccessToken();

        Cart = GetCart();
        ProductResponse product = CallGet<ProductResponse>($"https://localhost:7176/Product/{productId}");
        Cart.Add(product, total);

        SaveToSession("cart", Cart);
        return Page();
    }


    public IActionResult OnPostRemove(int productId)
    {
        OnGet();
        GetAccessToken();

        Cart = GetCart();
        Cart.Remove(productId);

        SaveToSession("cart", Cart);
        return Page();
    }

    public IActionResult OnPostOrder(string email, DateTime orderDate)
    {
        GetAccessToken();
        return TakeAction(() =>
        {
            Cart = GetCart();
            CallPost<None>($"https://localhost:7176/Order/Purchase", new PurchaseRequest { Cart = Cart, OrderDate = orderDate, CustomerId = _GetCustomerId(), Email = email});
            RemoveFromSession("cart");

            if (HasLogin())
            {
                return RedirectToPage("/account/orders");
            }

            return RedirectToPage("/index");
        });
    }

    private string _GetCustomerId()
    {
        AccountResponse? account = GetCurrentUser();
        if (account != null)
        {
            return account.CustomerId;
        }
        
        var request = new NewGuestCustomerRequest()
        {
            CompanyName = Customer.CompanyName,
            ContactName = Customer.ContactName,
            ContactTitle = Customer.ContactTitle,
            Address = Customer.Address,
        };
        
        return CallPost<CustomerResponse>($"https://localhost:7176/Customer/Guest", request).CustomerId;
    }

}