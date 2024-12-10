using SurveyPortal.Models;

namespace SurveyPortal.Models
{
    public class Answers
    {
        public int Id { get; set; } // Primary Key
        public string Response { get; set; } = string.Empty; // Kullanıcı cevabı
        public int QuestionId { get; set; } // Foreign Key
        public Question? Question { get; set; }  // Navigation Property
    }
}
