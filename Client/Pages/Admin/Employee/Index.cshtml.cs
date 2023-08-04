using API.Entities;
using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Admin.Employee
{
    public class IndexModel : ClientModel
    {
        [BindProperty]
        public String? Name { get; set; }

        [BindProperty]
        public ListResult<EmployeeResponse> ListEmployee { get; set; }
        public async Task<IActionResult> OnGet(String? name, int currentPage = 1, int size = 12, bool asc = true, string orderBy = "CreatedAt")
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }
            Name = name;
            ListEmployee = CallGet<ListResult<EmployeeResponse>>($"https://localhost:7176/Employee?page={currentPage}&size={size}&isAscending{asc}&orderBy={orderBy}&Name={name}");
            return Page();
        }
    }
}
