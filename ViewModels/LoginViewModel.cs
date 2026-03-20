using System.ComponentModel.DataAnnotations;

namespace Electrical_Equipment_Rental.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
