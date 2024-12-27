using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SurveyPortal.Repositories;
using SurveyPortal.Models;
using SurveyPortal.Data; // SeedData sýnýfýný kullanmak için ekledik
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging; // Loglama için ekledik
using SurveyPortal.Hubs; // SignalR hub'ýný ekleyin

var builder = WebApplication.CreateBuilder(args);

// Loglama mekanizmasýný ekliyoruz
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Veritabaný baðlantýsýný yapýyoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapýlandýrmasý
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // ApplicationUser kullanýldý
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Token iþlemleri için (örneðin, þifre sýfýrlama)

// SignalR'ý ekleyin
builder.Services.AddSignalR();

// MVC ve Controller/Views ekliyoruz
builder.Services.AddControllersWithViews();

// UnitOfWork ve Repository sýnýflarýný DI konteynerine ekliyoruz
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic Repository

// Session'ý ekliyoruz
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Cookie'yi güvenli yap
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// HTTPS yönlendirme
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS yönlendirmeyi aktifleþtiriyoruz
app.UseHttpsRedirection();

// Statik dosyalarý kullanmak için
app.UseStaticFiles();

// Routing
app.UseRouting();

// Session kullanýmý
app.UseSession();

// Kimlik doðrulama eklemek için
app.UseAuthentication();
app.UseAuthorization();

// SignalR endpoint'ini ekleyin
app.MapHub<UserHub>("/userHub");

// SeedData çalýþtýrma
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services); // Rolleri ve Admin kullanýcýsýný oluþturur
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "SeedData çalýþtýrýlýrken bir hata oluþtu.");
    }
}

// Controller ve View yönlendirmeleri
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();