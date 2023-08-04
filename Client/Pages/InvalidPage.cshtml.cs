using Client.Models;

namespace Client.Pages
{
    public class InvalidPageModel : ClientModel
    {
        public void OnGet()
        {
            GetAccessToken();
        }
    }
}
