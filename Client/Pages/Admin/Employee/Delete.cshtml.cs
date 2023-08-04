using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Admin.Employee
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
                CallDelete<EmployeeResponse>($"https://localhost:7176/Employee/{id}");
                return RedirectToPage("/Admin/Employee/Index");
            }, (ex) => RedirectToPage("/Admin/Employee/Index")
            );
        }
    }
}
