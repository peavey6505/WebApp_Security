using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood.Security;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
           await HttpContext.SignOutAsync(AuthSchemeNames.Cookie);
            return RedirectToPage("/Index");
        }
    }
}
