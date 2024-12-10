using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SurveyPortal.Controllers
{
    public class LoginController : Controller
    {
        // Login sayfasını görüntüle
        public IActionResult Index()
        {
            return View();
        }

        // Kullanıcı adı ve şifre ile login olma
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (username == "admin" && password == "password") // Basit admin kontrolü
            {
                HttpContext.Session.SetString("UserRole", "Admin"); // Oturum açma
                return RedirectToAction("Index", "Survey"); // Admin paneline yönlendirme
            }
            else
            {
                ViewBag.Message = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
        }

        // Çıkış yapma
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserRole"); // Oturumu kapat
            return RedirectToAction("Index", "Login"); // Giriş sayfasına yönlendirme
        }
    }

}
