using Newtonsoft.Json;
using SistemaMaritimo.Web.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SistemaMaritimo.Web.Services
{
    public class OrdenesServicioService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdenesServicioService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<OrdenServicioListItemViewModel>> ObtenerTodasAsync(string? barcoId = null, string? tecnicoId = null)
        {
            SetAuthorizationHeader();

            var url = $"{GetBaseUrl()}/api/ordenesservicio";
            var parametros = new List<string>();

            if (!string.IsNullOrWhiteSpace(barcoId))
                parametros.Add($"barcoId={barcoId}");

            if (!string.IsNullOrWhiteSpace(tecnicoId))
                parametros.Add($"tecnicoId={tecnicoId}");

            if (parametros.Any())
                url += "?" + string.Join("&", parametros);

            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<OrdenServicioListItemViewModel>>(json) ?? new();
        }

        public async Task<OrdenServicioViewModel?> ObtenerPorIdAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/ordenesservicio/{id}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<OrdenServicioViewModel>(json);
        }

        public async Task<List<TecnicoLookupViewModel>> ObtenerTecnicosAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/ordenesservicio/tecnicos");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<TecnicoLookupViewModel>>(json) ?? new();
        }

        public async Task<List<HistorialTecnicoViewModel>> ObtenerHistorialAsync(int barcoId)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/ordenesservicio/historial/{barcoId}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<HistorialTecnicoViewModel>>(json) ?? new();
        }

        public async Task<(bool ok, string mensaje)> CrearAsync(OrdenServicioViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/ordenesservicio", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? (true, body) : (false, body);
        }

        public async Task<(bool ok, string mensaje)> EditarAsync(OrdenServicioViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/ordenesservicio/{model.Id}", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? (true, body) : (false, body);
        }

        public async Task<(bool ok, string mensaje)> AsignarTecnicoAsync(int ordenId, int personalId)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(new { personalId });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/ordenesservicio/{ordenId}/asignar", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? (true, body) : (false, body);
        }

        public async Task<(bool ok, string mensaje)> CambiarEstadoAsync(int ordenId, string nuevoEstado)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(new { nuevoEstado });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/ordenesservicio/{ordenId}/estado", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? (true, body) : (false, body);
        }

        public async Task<(bool ok, string mensaje)> CerrarOrdenAsync(int ordenId, string informeCierre, string usuarioCierre)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(new { informeCierre, usuarioCierre });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/ordenesservicio/{ordenId}/cerrar", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? (true, body) : (false, body);
        }
    }
}