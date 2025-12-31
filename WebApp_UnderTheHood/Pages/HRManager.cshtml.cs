using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
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
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            var res = await httpClient.PostAsJsonAsync("auth", new { Username = "asd", Password = "asd" });
            res.EnsureSuccessStatusCode();
            string? jwt = await res.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(jwt);

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.AccessToken);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast");
        }
    }
}
