using DinkToPdf;
using DinkToPdf.Contracts;

public class Startup
{
    // Este método configura los servicios que la aplicación utilizará.
    public void ConfigureServices(IServiceCollection services)
    {
        // Registra el servicio IConverter
        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

    }

    // Este método configura la pipeline de solicitud HTTP.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
