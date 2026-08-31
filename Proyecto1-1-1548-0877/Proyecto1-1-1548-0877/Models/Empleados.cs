using System.ComponentModel.DataAnnotations;

namespace Proyecto1_1_1548_0877.Models
{
    public class Empleados
    {
        [Key]
        [Display(Name = "Número de cédula")]
        [Required(ErrorMessage = "La cédula es obligatoria")]
        public string Cedula { get; set; } = string.Empty;

        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        public DateTime FechaIngreso { get; set; }

        [Display(Name = "Salario por día")]
        [Required(ErrorMessage = "El campo salario es obligatorio")]
        public decimal SalarioDia { get; set; }

        [Display(Name = "Fecha de retiro")]
        [DataType(DataType.Date)]
        public DateTime? FechaRetiro { get; set; } 

        [Display(Name = "Tipo de empleado")]
        [Required(ErrorMessage = "El tipo de empleado es obligatorio")]
        public TipoEmpleado TipoEmpleado { get; set; }
    }

    //enumeration
    public enum TipoEmpleado
    {
        Veterinario,
        Asistente,
        Administrativo,
        Mantenimiento,
        Groomer
    }
}
    
