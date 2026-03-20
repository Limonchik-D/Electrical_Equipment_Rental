using System.ComponentModel.DataAnnotations;
using Electrical_Equipment_Rental.Models;

namespace Electrical_Equipment_Rental.ViewModels;

public class RentalCreateViewModel
{
    public int ProductUnitId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public decimal DepositAmount { get; set; }

    [Required(ErrorMessage = "Укажите дату возврата")]
    [Display(Name = "Планируемая дата возврата")]
    public DateTime PlannedEndAt { get; set; } = DateTime.Now.AddHours(2);

    [Required(ErrorMessage = "Выберите точку получения")]
    [Display(Name = "Точка получения")]
    public int StartLocationId { get; set; }

    [Display(Name = "Примечания")]
    public string? Notes { get; set; }

    public List<Location> Locations { get; set; } = [];
}
