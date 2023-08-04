using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class Profile : ClientModel
{
    public AccountResponse? Account { get; set; }
    public IActionResult OnGet()
    {
        Account = GetCurrentUser();
        if (Account == null)
        {
            return ToForbiddenPage();
        }
        return Page();
    }
}