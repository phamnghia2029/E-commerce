using API.Models.Requests;
using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class UpdateProfile : ClientModel
{

    [BindProperty]
    public UpdateProfileRequest Request { get; set; }

    public IActionResult OnGet()
    {
        if (!HasLogin())
        {
            return ToForbiddenPage();
        }
        AccountResponse? account = GetCurrentUser();
        StoreTempData("", account, account.Customer);
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
            CallPatch<None>($"https://localhost:7176/Account/{GetCurrentUser().AccountId}/Profile", Request);
            return RedirectToPage("/account/profile");
        });
    }
}