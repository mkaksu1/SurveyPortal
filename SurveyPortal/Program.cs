using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SurveyPortal.Repositories;
using SurveyPortal.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný yapýyoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC ve Controller/Views ekliyoruz
builder.Services.AddControllersWithViews();  // Burada sadece bir kez çaðrýldýðýndan emin ol

// UnitOfWork ve Repository sýnýflarýný DI konteynerine ekliyoruz
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic Repository

// **Session'ý ekliyoruz**
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
app.UseHttpsRedirection();  // HTTPS kullanmak için bu satýrý ekleyin

// Statik dosyalarý kullanmak için
app.UseStaticFiles();

// Routing
app.UseRouting();

// **Session'ý kullanmak için buraya ekliyoruz**
app.UseSession(); // Session kullanýmý

// Kimlik doðrulama eklemek için
app.UseAuthentication();  // Eðer kullanýcý doðrulamasý gerekiyorsa
app.UseAuthorization();   // Kullanýcý yetkilendirmesi

// Controller ve View yönlendirmeleri
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
