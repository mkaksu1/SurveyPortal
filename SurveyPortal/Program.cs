using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SurveyPortal.Repositories;
using SurveyPortal.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�yoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC ve Controller/Views ekliyoruz
builder.Services.AddControllersWithViews();  // Burada sadece bir kez �a�r�ld���ndan emin ol

// UnitOfWork ve Repository s�n�flar�n� DI konteynerine ekliyoruz
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic Repository

// **Session'� ekliyoruz**
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
app.UseHttpsRedirection();  // HTTPS kullanmak i�in bu sat�r� ekleyin

// Statik dosyalar� kullanmak i�in
app.UseStaticFiles();

// Routing
app.UseRouting();

// **Session'� kullanmak i�in buraya ekliyoruz**
app.UseSession(); // Session kullan�m�

// Kimlik do�rulama eklemek i�in
app.UseAuthentication();  // E�er kullan�c� do�rulamas� gerekiyorsa
app.UseAuthorization();   // Kullan�c� yetkilendirmesi

// Controller ve View y�nlendirmeleri
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
