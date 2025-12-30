using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            var client = _httpClientFactory.CreateClient("OurWebAPI");
            weatherForecastItems = await client.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast");
        }
    }
}
