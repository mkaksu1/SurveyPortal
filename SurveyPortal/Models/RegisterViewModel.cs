using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; } // Nullable yapıldı

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; } // Nullable yapıldı

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string? ConfirmPassword { get; set; } // Nullable yapıldı
}