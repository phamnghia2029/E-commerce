using API.Entities;
using API.Models.Requests.Post;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client.Pages.Admin.Employee
{
    public class CreateModel : ClientModel
    {
        [BindProperty]
        public NewEmployeeRequest Request { get; set; }
        public SelectList DepartmentIds;
        public async Task<IActionResult> OnGet()
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            DepartmentIds = new SelectList(CallGet<List<Department>>($"https://localhost:7176/Department"), "DepartmentId", "DepartmentName");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            DepartmentIds = new SelectList(CallGet<List<Department>>($"https://localhost:7176/Department"), "DepartmentId", "DepartmentName");

            return TakeAction(() =>
            {
                CallPost<EmployeeResponse>($"https://localhost:7176/Employee", Request);
                return RedirectToPage("/Admin/Employee/Index");
            });

        }
    }
}
