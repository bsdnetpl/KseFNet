using KseF.Models;
using KseF.Service;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
        Title = "KSeF API",
        Version = "v1",
        Description = "API for KSeF integration"
        });
    c.SchemaFilter<DefaultSchemaFilter>();
});
// Rejestracja us³ugi z konfiguracj¹
builder.Services.AddScoped<IKSeFClientService, KSeFClientService>(sp =>
{
    var config = sp.GetRequiredService<IOptions<KSeFConfig>>().Value;
    return new KSeFClientService(config.ApiUrl, config.Nip, config.ApiKey, config.PublicKeyPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
    }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
