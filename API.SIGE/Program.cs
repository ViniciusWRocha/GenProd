using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Repositories;
using API.SIGE.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");

// Database
builder.Services.AddDbContext<AppDbData>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();
builder.Services.AddScoped<ICaixilhoRepository, CaixilhoRepository>();
builder.Services.AddScoped<IFamiliaCaixilhoRepository, FamiliaCaixilhoRepository>();
builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<IUsuarioCargoRepository, UsuarioCargoRepository>();
builder.Services.AddScoped<IMedicaoRepository, MedicaoRepository>();
builder.Services.AddScoped<IProducaoFamiliaRepository, ProducaoFamiliaRepository>();
builder.Services.AddScoped<IAnexoRepository, AnexoRepository>();
builder.Services.AddScoped<INotificacaoRepository, NotificacaoRepository>();

// Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICaixilhoService, CaixilhoService>();
builder.Services.AddScoped<IObraService, ObraService>();
builder.Services.AddScoped<IFamiliaCaixilhoService, FamiliaCaixilhoService>();
builder.Services.AddScoped<ITipoUsuarioService, TipoUsuarioService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<IMedicaoService, MedicaoService>();
builder.Services.AddScoped<IProducaoFamiliaService, ProducaoFamiliaService>();
builder.Services.AddScoped<IAnexoService, AnexoService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("MyPolicy");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
