using API.Models;
using API.Models.Requests;
using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class SignUp : ClientModel
{

    [BindProperty] public SignUpRequest Account { get; set; }

    public IActionResult OnGet()
    {
        if (HasLogin())
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        return TakeAction(() =>
        {
            CallPost<None>("https://localhost:7176/Account/signup", Account);
            return RedirectToPage("/Account/Signin");
        });
    }
}