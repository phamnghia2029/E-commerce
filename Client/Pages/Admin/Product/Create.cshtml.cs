using API.Entities;
using API.Models.Requests;
using API.Models.Requests.Post;
using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Client.Pages.Admin.Product
{
    public class CreateModel : ClientModel
    {
        [BindProperty]
        public NewProductRequest Request { get; set; }
        public SelectList CategoryIds;
        public async Task<IActionResult> OnGet()
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            CategoryIds = new SelectList(CallGet<List<Category>>($"https://localhost:7176/Category"), "CategoryId", "CategoryName");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            CategoryIds = new SelectList(CallGet<List<Category>>($"https://localhost:7176/Category"), "CategoryId", "CategoryName");

            return TakeAction(() =>
            {
                CallPost<ProductResponse>($"https://localhost:7176/Product", Request);
                return RedirectToPage("/Admin/Product/Index");
            });
            
        }
    }
}
