using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Admin.Order
{
    public class CancelModel : ClientModel
    {
        public IActionResult OnGet(int id)
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            CallPatch<None>($"https://localhost:7176/Order/{id}/cancel");
            return RedirectToPage("/Admin/Order/Index");
        }
    }
}
