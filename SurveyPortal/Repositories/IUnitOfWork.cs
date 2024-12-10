using SurveyPortal.Models;

namespace SurveyPortal.Repositories
{
    // Repositories/IUnitOfWork.cs
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Survey> Surveys { get; }
        IRepository<Question> Questions { get; }
        IRepository<Answers> Answers { get; }

        void Save(); // Veritabanı değişikliklerini kaydeder
    }

}
