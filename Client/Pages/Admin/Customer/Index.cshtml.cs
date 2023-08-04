using API.Models.Domain;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;

namespace Client.Pages.Admin.Customer
{
    public class IndexModel : ClientModel
    {
        [BindProperty]
        public String? CustomerContactName { get; set; }

        [BindProperty]
        public ListResult<CustomerResponse> ListCustomer { get; set; }
        public async Task<IActionResult> OnGet(String? ContactName, int currentPage = 1, int size = 12, bool asc = true, string orderBy = "CreatedAt")
        {
            if (!IsAdmin())
            {
                return ToForbiddenPage();
            }

            CustomerContactName = ContactName;
            ListCustomer = CallGet<ListResult<CustomerResponse>>($"https://localhost:7176/Customer?page={currentPage}&size={size}&isAscending{asc}&orderBy={orderBy}&ContactName={ContactName}");
            return Page();
        }
    }
}

