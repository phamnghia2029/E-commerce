using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Product;

public class Detail : ClientModel
{
    public AccountResponse Account;

    public ProductResponse Product { get; set; }


    [HttpGet("{id}", Name = "OnGet")]
    public IActionResult OnGet(int id)
    {
        GetAccessToken();
        try
        {
            Product = CallGet<ProductResponse>($"https://localhost:7176/Product/{id}");
            return Page();
        }
        catch (Exception e)
        {
            return ToNotFoundPage();
        }
    }

    public IActionResult OnPostBuy(int productId)
    {
        GetAccessToken();
        try
        {
            Product = CallGet<ProductResponse>($"https://localhost:7176/Product/{productId}");
            Cart cart = GetCart();
            cart.Add(Product);
            SaveToSession("cart", cart);
            return RedirectToPage("/account/cart");
        }
        catch
        {
            return ToNotFoundPage();
        }
    }

    public IActionResult OnPostAdd(int productId)
    {
        GetAccessToken();
        try
        {

            Product = CallGet<ProductResponse>($"https://localhost:7176/Product/{productId}");
            Cart cart = GetCart();
            cart.Add(Product);
            SaveToSession("cart", cart);

            return RedirectToPage("/product/detail/" + productId);
        }
        catch
        {
            return ToNotFoundPage();
        }
    }
}