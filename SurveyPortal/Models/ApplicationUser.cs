using Microsoft.AspNetCore.Identity;

namespace SurveyPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Ek özellikler ekleyebilirsiniz
        public string? FullName { get; set; } // Nullable yapıldı
        public string? PhoneNumber { get; set; } // Nullable yapıldı
        public bool? PhoneNumberConfirmed { get; set; } // Nullable yapıldı
        public bool? TwoFactorEnabled { get; set; } // Nullable yapıldı
        public DateTimeOffset? LockoutEnd { get; set; } // Nullable yapıldı
        public bool? LockoutEnabled { get; set; } // Nullable yapıldı
        public int? AccessFailedCount { get; set; } // Nullable yapıldı
    }   
}