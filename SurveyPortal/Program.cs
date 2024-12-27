using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SurveyPortal.Repositories;
using SurveyPortal.Models;
using SurveyPortal.Data; // SeedData s�n�f�n� kullanmak i�in ekledik
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging; // Loglama i�in ekledik
using SurveyPortal.Hubs; // SignalR hub'�n� ekleyin

var builder = WebApplication.CreateBuilder(args);

// Loglama mekanizmas�n� ekliyoruz
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Veritaban� ba�lant�s�n� yap�yoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yap�land�rmas�
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // ApplicationUser kullan�ld�
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Token i�lemleri i�in (�rne�in, �ifre s�f�rlama)

// SignalR'� ekleyin
builder.Services.AddSignalR();

// MVC ve Controller/Views ekliyoruz
builder.Services.AddControllersWithViews();

// UnitOfWork ve Repository s�n�flar�n� DI konteynerine ekliyoruz
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic Repository

// Session'� ekliyoruz
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.HttpOnly = true; // Cookie'yi g�venli yap
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// HTTPS y�nlendirme
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS y�nlendirmeyi aktifle�tiriyoruz
app.UseHttpsRedirection();

// Statik dosyalar� kullanmak i�in
app.UseStaticFiles();

// Routing
app.UseRouting();

// Session kullan�m�
app.UseSession();

// Kimlik do�rulama eklemek i�in
app.UseAuthentication();
app.UseAuthorization();

// SignalR endpoint'ini ekleyin
app.MapHub<UserHub>("/userHub");

// SeedData �al��t�rma
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services); // Rolleri ve Admin kullan�c�s�n� olu�turur
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "SeedData �al��t�r�l�rken bir hata olu�tu.");
    }
}

// Controller ve View y�nlendirmeleri
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();