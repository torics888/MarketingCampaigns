
using MarketingCampaigns.Library.Impl.Extensions;
using MarketingCampaigns.Infrastructure.Impl.Extensions;
using MarketingCampaigns.Infrastructure.Impl.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cargar la configuración que está dentro de Configuration.
builder.Configuration.AddJsonFile(
    "Configuration/appsettings.json",
    optional: false,
    reloadOnChange: true);

// Crear una ruta estable dentro del proyecto de la API.
var dataDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Data");

Directory.CreateDirectory(dataDirectory);

var databasePath = Path.Combine(dataDirectory, "campaigns.db");

builder.Configuration["ConnectionStrings:DefaultConnection"] =
    $"Data Source={databasePath}";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLibraryServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Aplicar las migraciones pendientes al iniciar la aplicación.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<CampaignDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
