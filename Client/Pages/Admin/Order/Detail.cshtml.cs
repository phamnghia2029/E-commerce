using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Admin.Order
{
    public class DetailModel : ClientModel
    {
        [BindProperty]
        public List<OrderResponse> OrdersList { get; set; }
        public IActionResult OnGet(int id)
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            OrdersList = CallGet<List<OrderResponse>>($"https://localhost:7176/Order/detail/{id}").ToList();
            return Page();
        }
    }
}
