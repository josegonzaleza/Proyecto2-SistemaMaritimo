using Newtonsoft.Json;
using SistemaMaritimo.Web.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SistemaMaritimo.Web.Services
{
    public class PersonalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PersonalService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<PersonalViewModel>> ObtenerTodosAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/personal");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<PersonalViewModel>>(json) ?? new();
        }

        public async Task<PersonalViewModel?> ObtenerPorIdAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/personal/{id}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<PersonalViewModel>(json);
        }

        public async Task<PersonalDetalleViewModel?> ObtenerDetalleAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/personal/{id}/detalle");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<PersonalDetalleViewModel>(json);
        }

        public async Task<(bool ok, string mensaje)> CrearAsync(PersonalViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/personal", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Personal creado correctamente")
                : (false, body);
        }

        public async Task<(bool ok, string mensaje)> EditarAsync(PersonalViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/personal/{model.Id}", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Personal actualizado correctamente")
                : (false, body);
        }

        public async Task<bool> DesactivarAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"{GetBaseUrl()}/api/personal/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<LicenciaViewModel>> ObtenerLicenciasAsync(int personalId)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/personal/{personalId}/licencias");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<LicenciaViewModel>>(json) ?? new();
        }

        public async Task<bool> CrearLicenciaAsync(LicenciaViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/personal/{model.PersonalId}/licencias", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarLicenciaAsync(int personalId, int licenciaId)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"{GetBaseUrl()}/api/personal/{personalId}/licencias/{licenciaId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<AsignacionTripulacionViewModel>> ObtenerAsignacionesAsync(int personalId)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/personal/{personalId}/asignacionestripulacion");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<AsignacionTripulacionViewModel>>(json) ?? new();
        }

        public async Task<bool> CrearAsignacionAsync(AsignacionTripulacionViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/personal/{model.PersonalId}/asignacionestripulacion", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<BarcoLookupViewModel>> ObtenerBarcosAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/personal/barcos");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<BarcoLookupViewModel>>(json) ?? new();
        }
    }
}