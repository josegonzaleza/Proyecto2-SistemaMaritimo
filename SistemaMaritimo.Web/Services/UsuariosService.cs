using Newtonsoft.Json;
using SistemaMaritimo.Web.Models;
using System.Text;

namespace SistemaMaritimo.Web.Services
{
    public class UsuariosService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuariosService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> ActivarUsuarioAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.PutAsync(
                $"{GetBaseUrl()}/api/usuarios/{id}/activar",
                null
            );

            return response.IsSuccessStatusCode;
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
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<UsuarioViewModel>> ObtenerUsuariosAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/usuarios");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new List<UsuarioViewModel>();

            return JsonConvert.DeserializeObject<List<UsuarioViewModel>>(json) ?? new List<UsuarioViewModel>();
        }

        public async Task<List<RolViewModel>> ObtenerRolesAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/roles");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new List<RolViewModel>();

            return JsonConvert.DeserializeObject<List<RolViewModel>>(json) ?? new List<RolViewModel>();
        }

        public async Task<bool> EliminarUsuarioAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.DeleteAsync($"{GetBaseUrl()}/api/usuarios/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<UsuarioEditViewModel?> ObtenerUsuarioPorIdAsync(int id)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/usuarios/{id}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonConvert.DeserializeObject<UsuarioEditViewModel>(json);
        }

        public async Task<(bool ok, string mensaje)> CrearUsuarioAsync(UsuarioCreateViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GetBaseUrl()}/api/usuarios", content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (true, "Usuario creado correctamente");

            return (false, body);
        }


        public async Task<(bool ok, string mensaje)> EditarUsuarioAsync(UsuarioEditViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/usuarios/{model.Id}", content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (true, "Usuario actualizado correctamente");

            return (false, body);
        }

        public async Task<(bool ok, string mensaje)> CambiarPasswordAsync(CambiarPasswordViewModel model)
        {
            SetAuthorizationHeader();

            var json = JsonConvert.SerializeObject(new { nuevaClave = model.NuevaClave });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/usuarios/{model.Id}/password", content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (true, "Contraseña actualizada correctamente");

            return (false, body);
        }


        public async Task<List<BitacoraCambioRolViewModel>> ObtenerBitacoraAsync()
        {
            SetAuthorizationHeader();

            var response = await _httpClient.GetAsync($"{GetBaseUrl()}/api/usuarios/bitacora");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new List<BitacoraCambioRolViewModel>();

            return JsonConvert.DeserializeObject<List<BitacoraCambioRolViewModel>>(json) ?? new();
        }

        public async Task<bool> CambiarRolAsync(int usuarioId, int rolId, string realizadoPor)
        {
            SetAuthorizationHeader();

            var objeto = new
            {
                rolId = rolId,
                realizadoPor = realizadoPor
            };

            var json = JsonConvert.SerializeObject(objeto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetBaseUrl()}/api/usuarios/{usuarioId}/rol", content);
            return response.IsSuccessStatusCode;
        }
    }
}