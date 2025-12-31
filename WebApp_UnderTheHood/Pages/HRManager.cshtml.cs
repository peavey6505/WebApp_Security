using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json.Serialization;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "HRmanagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<WeatherForecastDto>? weatherForecastItems { get; set; }

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGet()
        {

            // get token from session
            JwtToken token = new JwtToken();

            var strTokenObj = HttpContext.Session.GetString("access_token");
            if(string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authienticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
            }

            if(token == null || string.IsNullOrEmpty(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authienticate();
            }

            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            httpClient.DefaultRequestHeaders.Authorization =
                   new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.AccessToken);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast");

        }

        private async Task<JwtToken> Authienticate()
        {
            //authentication and getting the token
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            var res = await httpClient.PostAsJsonAsync("auth", new { Username = "asd", Password = "asd" });
            res.EnsureSuccessStatusCode();
            string? jwt = await res.Content.ReadAsStringAsync();

            //store cookie in session
            HttpContext.Session.SetString("access_token", jwt);
            return JsonConvert.DeserializeObject<JwtToken>(jwt) ?? new JwtToken();
        }
    }
}
