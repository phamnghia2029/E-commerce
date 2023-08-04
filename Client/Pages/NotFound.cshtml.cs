using Client.Models;

namespace Client.Pages
{
    public class NotFoundModel : ClientModel
    {
        public void OnGet()
        {
            GetAccessToken();
        }
    }
}
