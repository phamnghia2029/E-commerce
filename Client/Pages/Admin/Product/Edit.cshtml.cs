using API.Entities;
using API.Models.Requests.Post;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Client.Pages.Admin.Product
{
    public class EditModel : ClientModel
    {
        [BindProperty]
        public UpdateProductRequest Request { get; set; }
        public SelectList CategoryIds;
        public async Task<IActionResult> OnGet(int id)
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            CategoryIds = new SelectList(CallGet<List<Category>>($"https://localhost:7176/Category"), "CategoryId", "CategoryName");
            return TakeAction(() =>
            {
                Request = CallGet<UpdateProductRequest>($"https://localhost:7176/Product/{id}");
                StoreRequestParams();

                //CategoryIds = new SelectList(
                //CallGet<List<Category>>($"https://localhost:7176/Category").Select(x => new SelectListItem()
                //{
                //    Selected = Request.CategoryId.Equals(x.CategoryId),
                //    Text = x.CategoryName,
                //    Value = x.CategoryId.ToString()
                //}), "Value", "Text");

                ViewData["Discontinued"] = Request.Discontinued ;
                return Page();
            });
        }

        public async Task<IActionResult> OnPost()
        {
            CategoryIds = new SelectList(CallGet<List<Category>>($"https://localhost:7176/Category"), "CategoryId", "CategoryName");

            return TakeAction(() =>
            {
                CallPut<ProductResponse>($"https://localhost:7176/Product/{Request.ProductId}", Request);
                return RedirectToPage("/Admin/Product/Index");
            });
        }
    }
}
