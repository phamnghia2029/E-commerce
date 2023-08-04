using API.Entities;
using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Product;

public class CategoryModel : ClientModel
{
    public List<Category> Categories { get; set; } = new();

    public int CategoryId { get; set; }

    public ListResult<ProductResponse> Items { get; set; } = new();

    public void OnGet(int id, int currentPage = 1, int size = 12, bool asc = true, string orderBy = "ProductId")
    {
        CategoryId = id;
        Categories = CallGet<List<Category>>($"https://localhost:7176/Category");
        Items = CallGet<ListResult<ProductResponse>>($"https://localhost:7176/Product?page={currentPage}&size={size}&isAscending{asc}&orderBy={orderBy}&categoryId={id}");
    }
    public IActionResult OnPostIncrease(int productId, int total)
    {
        Cart cart = GetCart();
        ProductResponse product = CallGet<ProductResponse>($"https://localhost:7176/Product/{productId}");

        cart.Add(product, total);
        SaveToSession("cart", cart);
        return RedirectToPage("/account/cart");
    }
}