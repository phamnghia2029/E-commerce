using API.Entities;
using API.Models.Requests.Post;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client.Pages.Admin.Employee
{
    public class EditModel : ClientModel
    {
        [BindProperty]
        public UpdateEmployeeRequest Request { get; set; }
        public SelectList DepartmentIds;
        public async Task<IActionResult> OnGet(int id)
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }

            DepartmentIds = new SelectList(CallGet<List<Department>>($"https://localhost:7176/Department"), "DepartmentId", "DepartmentName");
            return TakeAction(() =>
            {
                Request = CallGet<UpdateEmployeeRequest>($"https://localhost:7176/Employee/{id}");
                StoreRequestParams();
                ViewData["BirthDates"] = DateTime.Parse(Request.BirthDate.ToString()).ToString("yyyy-MM-dd");
                ViewData["HireDates"] = DateTime.Parse(Request.HireDate.ToString()).ToString("yyyy-MM-dd");
                return Page();
            });
        }

        public async Task<IActionResult> OnPost()
        {
            DepartmentIds = new SelectList(CallGet<List<Department>>($"https://localhost:7176/Department"), "DepartmentId", "DepartmentName");

            return TakeAction(() =>
            {
                CallPut<EmployeeResponse>($"https://localhost:7176/Employee/{Request.EmployeeId}", Request);
                return RedirectToPage("/Admin/Employee/Index");
            });
        }
    }
}
