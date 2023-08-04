using API.Models.Requests;
using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class UpdatePassword : ClientModel
{
    [BindProperty]
    public UpdatePasswordRequest Request { get; set; }
    
    public IActionResult OnGet()
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }
        AccountResponse? account = GetCurrentUser();
        StoreTempData("", account);
        return Page();
    }
    
    public IActionResult OnPost()
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }

        return TakeAction(() =>
        {
            CallPatch<None>($"https://localhost:7176/Account/{GetCurrentUser().AccountId}/Password", Request);
            return RedirectToPage("/account/profile");
        });
    }
}