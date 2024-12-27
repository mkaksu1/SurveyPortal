using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SurveyPortal.Hubs;
using SurveyPortal.Models;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IHubContext<UserHub> _hubContext; // SignalR Hub'ı için ekledik

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger,
        IHubContext<UserHub> hubContext) // SignalR Hub'ını enjekte ettik
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _hubContext = hubContext;
    }

    // Giriş (Login) İşlemleri
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Kullanıcının rolünü kontrol et
            var user = await _userManager.FindByEmailAsync(email);
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Admin ise dashboard'a yönlendir
                return RedirectToAction("Dashboard", "Account");
            }
            else
            {
                // Normal kullanıcı ise anasayfaya yönlendir
                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
    }

    // Kayıt Ol (Register) İşlemleri
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model == null || model.Email == null || model.Password == null || model.ConfirmPassword == null)
        {
            ModelState.AddModelError(string.Empty, "Geçersiz form verisi.");
            return View(model);
        }

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Varsayılan olarak "User" rolünü ata
                await _userManager.AddToRoleAsync(user, "User");

                // SignalR üzerinden yeni kullanıcı bildirimi gönder
                await _hubContext.Clients.All.SendAsync("ReceiveNewUser", user.Id, user.UserName);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                // Hataları logla
                _logger.LogError($"Kayıt işlemi sırasında hata: {error.Description}");
            }
        }
        else
        {
            // ModelState hatalarını logla
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogError($"ModelState hatası: {error.ErrorMessage}");
                }
            }
        }

        return View(model);
    }

    // Admin Dashboard
    [Authorize(Roles = "Admin")]
    public IActionResult Dashboard()
    {
        return View();
    }
}