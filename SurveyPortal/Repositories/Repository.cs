using Microsoft.EntityFrameworkCore;
using SurveyPortal.Models;

namespace SurveyPortal.Repositories
{
    // Repositories/Repository.cs
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // DbSet'i almak için
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList(); // Tablodaki tüm verileri alır
        public T GetById(int id) => _dbSet.Find(id); // Id'ye göre veriyi bulur
        public void Insert(T entity) => _dbSet.Add(entity); // Yeni veriyi ekler
        public void Update(T entity) => _dbSet.Update(entity); // Veriyi günceller
        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null) _dbSet.Remove(entity); // Veriyi siler
        }
    }

}
