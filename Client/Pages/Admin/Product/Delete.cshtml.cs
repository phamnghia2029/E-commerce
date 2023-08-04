using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Admin.Product
{
    public class DeleteModel : ClientModel
    {
        public IActionResult OnGet(int id)
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            return TakeAction(() =>
            {
                CallDelete<ProductResponse>($"https://localhost:7176/Product/{id}");
                return RedirectToPage("/Admin/Product/Index");
            }, (ex) => RedirectToPage("/Admin/Product/Index")
            );
        }
    }
}
