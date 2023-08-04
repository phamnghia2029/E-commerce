using API.Models.Responses;
using Base.Functionals;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account
{
    public class SignOutModel : ClientModel
    {

        public IActionResult OnGet()
        {
            AccountResponse? user = GetCurrentUser();
            RemoveFromCookie("account");
            RemoveFromCookie("accessToken");
            RemoveFromCookie("refreshToken");
            string? fromCookie = GetFromCookie<string>("accessToken");

            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            return TakeAction(() =>
            {
                CallPost<None>($"https://localhost:7176/Account/Logout/{user.AccountId}");
                RemoveFromCookie("account");
                RemoveFromCookie("accessToken");
                RemoveFromCookie("refreshToken");
                return RedirectToPage("/Index");
            });
        }
    }
}
