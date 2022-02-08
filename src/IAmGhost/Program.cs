using IAmGhost.Interfaces;
using IAmGhost.Services;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.OpenApi.Models;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default,
};

var builder = WebApplication.CreateBuilder(options);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddEnvironmentVariables("I_AM_GHOST_");

var baseStorage = builder.Configuration.GetValue<string>("BaseStoragePath");

// Add services to the container.
builder.Services.AddSingleton<IGhostService>(v => new JsonFileGhostService(baseStorage));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "IAmGhost Snapshot API", Version = "v1" });
});

if(!args.Any(arg => arg.Equals("--console")))
{
    builder.Host.UseWindowsService(options => options.ServiceName = "I Am Ghost");
}

builder.WebHost.UseUrls("http://localhost:7012/");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
