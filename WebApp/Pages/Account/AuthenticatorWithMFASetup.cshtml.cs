using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public SetupMFAViewModel ViewModel { get; set; }

        [BindProperty]
        public bool Succeeded { get; set; }

        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            _userManager = userManager;
            ViewModel = new SetupMFAViewModel();
            Succeeded = false;
        }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                var key = await _userManager.GetAuthenticatorKeyAsync(user);
                ViewModel.Key = key;
                ViewModel.QRCodeBytes = GenerateQRCodeBytes(
                    provider: "WebApp",
                    key: ViewModel.Key,
                    userEmail: user.Email ?? string.Empty);
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user != null && await _userManager.VerifyTwoFactorTokenAsync( //checks AspNetUserTokens for the 2FA token by user
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                ViewModel.SecurityCode))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                Succeeded = true;
            }
            else
            {
                ModelState.AddModelError("SetupMFA", "Something went wrong ");
            }

            return Page();
        }

        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(
                $"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",
                QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }

    public class SetupMFAViewModel
    {
        public string? Key { get; set; }
        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; } = string.Empty;

        public Byte[]? QRCodeBytes { get; set; }
    }
}
