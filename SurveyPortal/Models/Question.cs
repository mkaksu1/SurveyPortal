using SurveyPortal.Models;

namespace SurveyPortal.Models
{
    public class Question
    {
        public int Id { get; set; } // Primary Key
        public string Text { get; set; } = string.Empty; // Soru metni + VARSAYILAN DEGER
        public int SurveyId { get; set; } // Foreign Key
        public Survey? Survey { get; set; }  // Navigation Property - NULLABLE
        public ICollection<Answers> Answers { get; set; } // Answers ile ilişki
    }
}
