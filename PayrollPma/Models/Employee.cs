using System.ComponentModel.DataAnnotations;

namespace PayrollPma.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "La rata por hora debe ser mayor a cero.")]
    [Display(Name = "Rata por hora")]
    public decimal RatePerHour { get; set; }

    [Range(0, 720, ErrorMessage = "Las horas mensuales deben estar entre 0 y 720.")]
    [Display(Name = "Horas mensuales")]
    public int MonthlyHours { get; set; }
}
