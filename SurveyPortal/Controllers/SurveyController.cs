using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPortal.Models;
using SurveyPortal.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace SurveyPortal.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
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
            var surveys = _unitOfWork.Surveys.GetAll();
            return View(surveys);
        }

        // Yeni anket oluşturma (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Yeni anket oluşturma (POST)
        [HttpPost]
        public IActionResult Create(Survey survey)
        {
            if (ModelState.IsValid)
            {
                // Anketi ve soruları kaydet
                _unitOfWork.Surveys.Insert(survey);
                _unitOfWork.Save();

                return RedirectToAction("Index"); // Anket listesine yönlendir
            }

            return View(survey);
        }

        // Anket düzenleme (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
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
            if (ModelState.IsValid)
            {
                // Anketi ve soruları güncelle
                _unitOfWork.Surveys.Update(survey);
                _unitOfWork.Save();

                return RedirectToAction("Index"); // Anket listesine yönlendir
            }

            return View(survey);
        }

        // Anket silme (GET)
        [HttpGet]
        public IActionResult Delete(int id)
        {
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
            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey != null)
            {
                _unitOfWork.Surveys.Delete(id);
                _unitOfWork.Save(); // Değişiklikleri kaydet
            }
            return RedirectToAction("Index");
        }

        // Anket cevaplama sayfası (GET)
        [AllowAnonymous] // Herkes erişebilir
        [HttpGet]
        public IActionResult Answer(int id)
        {
            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // Anket cevaplarını kaydetme (POST)
        [AllowAnonymous] // Herkes erişebilir
        [HttpPost]
        public IActionResult Answer(int surveyId, List<string> responses)
        {
            var survey = _unitOfWork.Surveys.GetById(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            // Cevapları kaydet
            for (int i = 0; i < survey.Questions.Count; i++)
            {
                var answer = new Answers
                {
                    Response = responses[i],
                    QuestionId = survey.Questions.ToList()[i].Id
                };
                _unitOfWork.Answers.Insert(answer);
            }

            _unitOfWork.Save();
            return RedirectToAction("ThankYou"); // Teşekkür sayfasına yönlendir
        }

        // Anket sonuçlarını görüntüleme
        [HttpGet]
        public IActionResult Results(int id)
        {
            var survey = _unitOfWork.Surveys.GetById(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // Teşekkür sayfası
        [AllowAnonymous] // Herkes erişebilir
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}