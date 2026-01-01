using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public RegisterModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            // validate email (optional)

            //create user 
            var user = new User
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email
            };

            // populates AspNetUserClaims table
            var claimDepartment = new Claim("Department", RegisterViewModel.Department);
            var claimPosition = new Claim("Position", RegisterViewModel.Position);

            var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);
            if(result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, claimDepartment);
                await _userManager.AddClaimAsync(user, claimPosition);

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken }) ?? string.Empty);

            }
            else
            {
                foreach(var e in result.Errors)
                {
                    ModelState.AddModelError("Register", e.Description);
                }

                return Page();
            }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(dataType:DataType.Password)]
        public string Password { get; set; } =  string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
