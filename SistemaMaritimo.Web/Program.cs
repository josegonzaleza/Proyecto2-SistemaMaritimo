using SistemaMaritimo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<UsuariosService>();
builder.Services.AddHttpClient<PersonalService>();
builder.Services.AddHttpClient<BarcosService>();
builder.Services.AddHttpClient<TravesiasService>();
builder.Services.AddHttpClient<OrdenesServicioService>();
builder.Services.AddHttpClient<DashboardService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();