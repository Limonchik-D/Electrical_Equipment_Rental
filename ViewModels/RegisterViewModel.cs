using System.ComponentModel.DataAnnotations;

namespace Electrical_Equipment_Rental.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите имя")]
    [Display(Name = "Полное имя")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
