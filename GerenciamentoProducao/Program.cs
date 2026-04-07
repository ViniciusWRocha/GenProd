using GerenciamentoProducao.Configuration;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// API Settings
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// HttpClient para consumir a API
builder.Services.AddHttpClient("ApiSige", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    // Em desenvolvimento, aceitar certificado auto-assinado da API
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});

// Serviços de API Client (substituem os antigos repositórios EF)
builder.Services.AddScoped<IObraRepository, ObraApiService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioApiService>();
builder.Services.AddScoped<ICaixilhoRepository, CaixilhoApiService>();
builder.Services.AddScoped<IFamiliaCaixilhoRepository, FamiliaCaixilhoApiService>();
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioApiService>();
builder.Services.AddScoped<IDashboardApiService, DashboardApiService>();

// Autenticação com COOKIES
builder.Services.AddAuthentication("GerenciadorProd")
    .AddCookie("GerenciadorProd", options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.AccessDeniedPath = "/Usuario/AcessoNegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Add services to the container
builder.Services.AddControllersWithViews();

// Configuração de cultura para datas
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "pt-BR", "en-US" };
    options.SetDefaultCulture("pt-BR")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseHttpMethodOverride();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
