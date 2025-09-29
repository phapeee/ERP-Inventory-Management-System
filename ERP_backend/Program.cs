using ERP_backend.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Launch the app and navigate to https://localhost:<port>/openapi/v1.json to view the generated OpenAPI document.
builder.Services.AddOpenApi();

// Setup link: https://www.c-sharpcorner.com/article/building-a-powerful-asp-net-core-web-api-with-postgresql/
// Offical setup link: https://www.npgsql.org/efcore/index.html?tabs=onconfiguring
// Use .NET (C#) to connect and query data in Azure Database for PostgreSQL flexible server: https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/connect-csharp?utm_source=chatgpt.com
builder.Services.AddDbContext<AppDb>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();  // UI at /scalar
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
