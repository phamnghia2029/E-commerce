using API.Models.Requests;
using API.Models.Responses;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Pages.Account
{
    public class SigninModel : ClientModel
    {
        [BindProperty]
        public LoginRequest Account { get; set; }

        public void OnGet()
        {
            GetAccessToken();
        }

        public IActionResult OnPost()
        {
            return TakeAction(() =>
            {
                AuthUser user = CallPost<AuthUser>("https://localhost:7176/Account/login", Account);
                SaveToCookie("account", user);
                SaveToCookie("accessToken", user.AccessToken);
                SaveToCookie("refreshToken", user.RefreshToken);
                if (user.IsAdmin)
                {
                    return RedirectToPage("/Admin/Dashboard/Index");
                }
                return RedirectToPage("/Index");
            });
        }
    }
}
