using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authentication.Cookies;
using HDUA.Helpers;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var context = new CustomAssemblyLoadContext();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "LibreriaPDF", "libwkhtmltox.dll"));
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    context.LoadUnmanagedLibrary("/usr/lib/x86_64-linux-gnu/libwkhtmltox.so");
}

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opcion =>
    {
        opcion.LoginPath = "/Login/Login";
        opcion.ExpireTimeSpan = TimeSpan.FromMinutes(45);
        opcion.AccessDeniedPath = "/Principal/Principal";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();     

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();

