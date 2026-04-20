using Newtonsoft.Json;
using SistemaMaritimo.Web.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SistemaMaritimo.Web.Services
{
    public class BarcosService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BarcosService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetBaseUrl()
        {
            return _configuration["ApiSettings:BaseUrl"]!.TrimEnd('/');
        }

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

        public async Task<List<BarcoViewModel>> ObtenerTodosAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/barcos");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<BarcoViewModel>>(json) ?? new();
        }

        public async Task<BarcoViewModel?> ObtenerPorIdAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/barcos/{id}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<BarcoViewModel>(json);
        }

        public async Task<(bool ok, string mensaje)> CrearAsync(BarcoViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/barcos", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Barco creado correctamente")
                : (false, body);
        }

        public async Task<(bool ok, string mensaje)> EditarAsync(BarcoViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/barcos/{model.Id}", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Barco actualizado correctamente")
                : (false, body);
        }

        public async Task<bool> ArchivarAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"{GetBaseUrl()}/api/barcos/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActivarAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/barcos/{id}/activar", null);
            return response.IsSuccessStatusCode;
        }
    }
}