using API.Entities;
using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages
{
    public class IndexModel : ClientModel
    {
        public List<Category> Categories { get; set; } = new();

        public List<ProductResponse> BestSellerProducts { get; set; } = new();
        public List<ProductResponse> NewProducts { get; set; } = new();
        public void OnGet()
        {
            GetAccessToken();

            Categories = CallGet<List<Category>>($"https://localhost:7176/Category");
            BestSellerProducts = CallGet<List<ProductResponse>>($"https://localhost:7176/Product/Best?total=4");
            NewProducts = CallGet<List<ProductResponse>>($"https://localhost:7176/Product/Newest?total=4");
        }

        public IActionResult OnPostBuy(int productId)
        {
            GetAccessToken();
            try
            {
                ProductResponse product = CallGet<ProductResponse>($"https://localhost:7176/Product/{productId}");
                Cart cart = GetCart();
                cart.Add(product);
                SaveToSession("cart", cart);
                return RedirectToPage("/account/cart");
            }
            catch
            {
                return ToNotFoundPage();
            }
        }
    }
    
    
}