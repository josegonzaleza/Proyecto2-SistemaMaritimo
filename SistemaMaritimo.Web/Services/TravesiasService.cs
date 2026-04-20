using Newtonsoft.Json;
using SistemaMaritimo.Web.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SistemaMaritimo.Web.Services
{
    public class TravesiasService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TravesiasService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<TravesiaListItemViewModel>> ObtenerTodasAsync(string? barcoId = null, string? puertoDestino = null)
        {
            SetAuthorizationHeader();

            var url = $"{GetBaseUrl()}/api/travesias";
            var parametros = new List<string>();

            if (!string.IsNullOrWhiteSpace(barcoId))
                parametros.Add($"barcoId={barcoId}");

            if (!string.IsNullOrWhiteSpace(puertoDestino))
                parametros.Add($"puertoDestino={Uri.EscapeDataString(puertoDestino)}");

            if (parametros.Any())
                url += "?" + string.Join("&", parametros);

            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new();
            return JsonConvert.DeserializeObject<List<TravesiaListItemViewModel>>(json) ?? new();
        }

        public async Task<TravesiaViewModel?> ObtenerPorIdAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/travesias/{id}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return JsonConvert.DeserializeObject<TravesiaViewModel>(json);
        }

        public async Task<(bool ok, string mensaje)> CrearAsync(TravesiaViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/travesias", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Travesía creada correctamente")
                : (false, body);
        }

        public async Task<(bool ok, string mensaje)> EditarAsync(TravesiaViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/travesias/{model.Id}", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Travesía actualizada correctamente")
                : (false, body);
        }

        public async Task<(bool ok, string mensaje)> CambiarEstadoAsync(int id, string nuevoEstado, string usuario)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(new
            {
                nuevoEstado,
                usuario
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/travesias/{id}/estado", content);
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? (true, "Estado actualizado correctamente")
                : (false, body);
        }
    }
}