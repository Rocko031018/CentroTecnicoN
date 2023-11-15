using SistemaVenta.IOC;
using SistemaVenta.AplicacionWeb.Utilidades.Automapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using SistemaVenta.AplicacionWeb.Utilidades.Extensiones;

using SistemaVenta.AplicacionWeb.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.InyectarDependencia(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddDbContext<DbventaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));
var context = new CustomAssemblyLoadContext();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
