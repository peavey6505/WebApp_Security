using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; }

        public LoginTwoFactorModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            EmailMFA = new EmailMFA();
        }

        public async Task OnGet(string email, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            //_userManager.Users.FirstOrDefault(u => u.Email == email);

            EmailMFA.SecurityCode = string.Empty;
            EmailMFA.RememberMe = rememberMe;
            //Generate the code
            var securityCode  = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send to user, e.g. email
        }

        public async Task<IActionResult> OnPostAsync(string email, bool rememberMe)
        {

            if(!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", 
                EmailMFA.SecurityCode, 
                EmailMFA.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "User account is locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Invalid login attempt.");
                }

                return Page();
            }
        }
    }

    public class EmailMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
