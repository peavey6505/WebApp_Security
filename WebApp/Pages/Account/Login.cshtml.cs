using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Credential.Email, Credential.Password, Credential.RemeberMe, false);
            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if(result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginTwoFactor", new { Credential.Email, Credential.RemeberMe });
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "User account is locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Invalid login attempt.");
                }

                return Page();
            }
        }

    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")] 
        public bool RemeberMe { get; set; }
    }
}
