using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public AuthenticatorMFAViewModel AuthenticatorMFA { get; set; }

        public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
        {
            AuthenticatorMFA = new AuthenticatorMFAViewModel();
            _signInManager = signInManager;
        }
        public void OnGet(bool rememberMe)
        {
            AuthenticatorMFA.SecurityCode = string.Empty;
            AuthenticatorMFA.RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                AuthenticatorMFA.SecurityCode,
                AuthenticatorMFA.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "User account is locked out.");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Invalid login attempt.");
                }

                return Page();
            }
        }
    }

    public class AuthenticatorMFAViewModel
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
