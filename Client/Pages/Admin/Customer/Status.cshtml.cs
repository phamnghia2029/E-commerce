using API.Entities;
using API.Models.Domain;
using API.Models.Responses;
using Aspose.Pdf.Drawing;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;

namespace Client.Pages.Admin.Customer
{
    public class StatusModel : ClientModel
    {
        public async Task<IActionResult> OnGet(String id)
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }

            CallPut<None>($"https://localhost:7176/Customer/{id}/SetStatus");
            return RedirectToPage("/Admin/Customer/Index");
        }
    }
}
