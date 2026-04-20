using Newtonsoft.Json;
using SistemaMaritimo.Web.Models;
using System.Net.Http.Headers;

namespace SistemaMaritimo.Web.Services
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetBaseUrl() => _configuration["ApiSettings:BaseUrl"]!.TrimEnd('/');

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<DashboardViewModel?> ObtenerDashboardAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/dashboard");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<DashboardViewModel>(json);
        }
    }
}