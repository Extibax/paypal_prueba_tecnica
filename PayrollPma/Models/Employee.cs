using System.ComponentModel.DataAnnotations;

namespace PayrollPma.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "La rata por hora debe ser mayor a cero.")]
    public decimal RatePerHour { get; set; }

    [Range(0, 720, ErrorMessage = "Las horas mensuales deben estar entre 0 y 720.")]
    public int MonthlyHours { get; set; }
}
