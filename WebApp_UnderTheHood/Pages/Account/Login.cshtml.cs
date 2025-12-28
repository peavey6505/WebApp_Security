using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using WebApp_UnderTheHood.Security;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync() {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // verify the credential
            if(Credential.UserName == "asd" && Credential.Password == "asd")
            {
                // creating security context 
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@asd.asd"),
                    new Claim("Department", "HR"), //Custom claim for authorization policy
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true")
                };

                var identity = new ClaimsIdentity(claims, AuthSchemeNames.Cookie); // cookie auth, typical for login page

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);


                // create encrypted cookie with user claims
                // cookie goes to browser
                // with every next req browser sends it back
                // .net core knows that user is already logged in with this cookie
                await HttpContext.SignInAsync(AuthSchemeNames.Cookie, claimsPrincipal);

                return RedirectToPage("/Index");
            }

            return Page();

        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
