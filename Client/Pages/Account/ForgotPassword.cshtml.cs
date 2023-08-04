using API.Models.Requests;
using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account;

public class ForgotPassword : ClientModel
{
    [BindProperty]
    public ForgotPasswordRequest Request { get; set; }
    
    public void OnGet()
    {
        
    }
    
    public IActionResult OnPost()
    {
        if (HasLogin())
        {
            return RedirectToPage("/Index");
        }
        GetAccessToken();

        return TakeAction(() =>
        {
            CallPost<AuthUser>("https://localhost:7176/Account/ForgotPassword", Request);
            return RedirectToPage("/Account/Signin");
        });

    }
}