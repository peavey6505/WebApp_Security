using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebApp_UnderTheHood.Security;

namespace Web_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.Username == "asd" && credential.Password == "asd")
            {
                // creating security context 
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@asd.asd"),
                    new Claim("Department", "HR"), //Custom claim for authorization policy
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2025-05-05")
                };


                // JWT TOKEN contains only claims so we do not use this, as with cookies
                //var identity = new ClaimsIdentity(claims, AuthSchemeNames.Cookie); // cookie auth, typical for login page
                //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                //var authProperties = new AuthenticationProperties
                //{
                //    IsPersistent = Credential.RemeberMe
                //};


                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    acces_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt,
                });

            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
            var problemDetails = new ProblemDetails(){
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized
            };

            return Unauthorized(problemDetails);
        }

        private string CreateToken(List<Claim> claims, DateTime expiresAt)
        {
            var claimsDic = new Dictionary<string, object>();
            if(claims is not null && claims.Count > 0)
            {
                foreach(var claim in claims)
                {
                    claimsDic.Add(claim.Type, claim.Value);
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["SecretKey"] ?? string.Empty)),
                    SecurityAlgorithms.HmacSha256Signature),
                Claims = claimsDic,
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow,
            };
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }

    public class Credential
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
