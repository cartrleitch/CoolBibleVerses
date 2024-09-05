using CoolBibleVerses.Data;
using CoolBibleVerses.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

var logger = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()).CreateLogger("CertificateLogger");

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var certPath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:Path"];
    var certKeyPath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:KeyPath"];
    try
    {
        var certificate = new X509Certificate2(certPath, certKeyPath);
        serverOptions.ConfigureHttpsDefaults(listenOptions =>
        {
            listenOptions.ServerCertificate = certificate;
        });

        logger.LogInformation($"Certificate loaded successfully. Subject: {certificate.Subject}, Issuer: {certificate.Issuer}, Thumbprint: {certificate.Thumbprint}");
    }
    catch (Exception ex)
    {
        logger.LogError($"Failed to load certificate. Path: {certPath}, KeyPath: {certKeyPath}, Exception: {ex.Message}");
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    RolesSeed.Initialize(services, userManager).Wait();
}

app.Run();