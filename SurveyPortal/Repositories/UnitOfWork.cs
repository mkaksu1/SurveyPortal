using SurveyPortal.Models;

namespace SurveyPortal.Repositories
{
    // Repositories/UnitOfWork.cs
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<Survey> _surveys;
        private IRepository<Question> _questions;
        private IRepository<Answers> _answers;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Survey> Surveys => _surveys ??= new Repository<Survey>(_context);
        public IRepository<Question> Questions => _questions ??= new Repository<Question>(_context);
        public IRepository<Answers> Answers => _answers ??= new Repository<Answers>(_context);

        public void Save()
        {
            _context.SaveChanges(); // Veritabanındaki değişiklikleri kaydeder
        }

        public void Dispose()
        {
            _context.Dispose(); // Bağlantıyı kapatır
        }
    }

}
