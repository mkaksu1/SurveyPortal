namespace SurveyPortal.Repositories
{
    // Repositories/IRepository.cs
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();         // Tüm verileri al
        T GetById(int id);               // id'ye göre tek bir veri al
        void Insert(T entity);           // Yeni veri ekle
        void Update(T entity);           // Veriyi güncelle
        void Delete(int id);             // id'ye göre veriyi sil
    }
}