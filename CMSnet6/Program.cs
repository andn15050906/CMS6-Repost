using Microsoft.EntityFrameworkCore;

using CMSnet6.Models.EntityModels;
using CMSnet6.Models;
using CMSnet6.Services.Extensions;
using CMSnet6.Helpers;
using CMSnet6.Helpers.CookieConfig;



var builder = WebApplication.CreateBuilder(args);

Configurer.GetConfiguration();

// Add services to the container.

builder.Services.AddCors(o => o.AddPolicy("Policy", builder =>
    builder.WithOrigins(Configurer.GetCORS()).AllowCredentials().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();

string connectionString = Configurer.GetConnectionString();
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
builder.Services.AddScoped<UnitOfWork>();
AppStart.ExecuteWarmQuery(connectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddJwt(Configurer.GetJwtOptions());
CookieConfigurer.Create(Configurer.GetCookieConfigOptions());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Policy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();