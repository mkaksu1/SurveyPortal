using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyPortal.Models
{
    public class Survey
    {
        public int Id { get; set; }

        // Varsayılan boş değer atıyoruz
        public string Title { get; set; } = string.Empty;  // Null olamayacak, boş bir string
        public string Description { get; set; } = string.Empty;  // Aynı şekilde boş string

        public DateTime CreatedDate { get; set; } = DateTime.Now;  // Varsayılan tarih

        public ICollection<Question> Questions { get; set; } = new List<Question>();  // Default Collection
    }

}
