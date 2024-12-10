using Microsoft.AspNetCore.Mvc;
using SurveyPortal.Models;
using SurveyPortal.Repositories;
using Microsoft.AspNetCore.Http;

namespace SurveyPortal.Controllers
{
    public class SurveyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SurveyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Anketlerin listelenmesi
        public IActionResult Index()
        {
            // Admin kontrolü
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            var surveys = _unitOfWork.Surveys.GetAll();
            return View(surveys);
        }

        // Tek bir anketin detaylarının gösterilmesi
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // Yeni anket oluşturma (GET)
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            return View();
        }

        // Yeni anket oluşturma (POST)
        [HttpPost]
        public IActionResult Create(Survey survey)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Surveys.Insert(survey);
                _unitOfWork.Save(); // Değişiklikleri kaydet
                return RedirectToAction("Index");
            }
            return View(survey);
        }


        //AJAX METODU İÇİN
        [HttpPost]
        public JsonResult AddSurveyAjax([FromBody] Survey survey)
        {
            if (survey.Title == null || survey.Description == null)
            {
                return Json(new { success = false, message = "Başlık ve açıklama boş olamaz." });
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Surveys.Insert(survey);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Anket başarıyla eklendi!" });
            }

            return Json(new { success = false, message = "Anket eklenirken bir hata oluştu." });
        }



        // Anket düzenleme (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // Anket düzenleme (POST)
        [HttpPost]
        public IActionResult Edit(Survey survey)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Surveys.Update(survey);
                _unitOfWork.Save(); // Değişiklikleri kaydet
                return RedirectToAction("Index");
            }
            return View(survey);
        }

        // Anket silme (GET)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // Anket silme (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Login"); // Admin değilse login sayfasına yönlendir
            }

            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey != null)
            {
                _unitOfWork.Surveys.Delete(id);
                _unitOfWork.Save(); // Değişiklikleri kaydet
            }
            return RedirectToAction("Index");
        }
    }
}
