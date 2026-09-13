using System.ComponentModel.DataAnnotations;

namespace Proyecto2_1_1548_0877.Models
{
    public class Employes
    {
        [Key]
        [Display(Name = "ID number")]
        [Required(ErrorMessage = "The ID number is mandatory")]
        public string Id { get; set; } = string.Empty;



        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "The date of birth is mandatory")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "The start date is mandatory")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Salary per day")]
        [Required(ErrorMessage = "The salary field is mandatory")]
        public decimal SalaryPerDay { get; set; }

        [Display(Name = "Retirement date")]
        [DataType(DataType.Date)]
        public DateTime? RetirementDate { get; set; }

        [Display(Name = "Employee type")]
        [Required(ErrorMessage = "The employee type is mandatory")]
        public TypeEmploye TypeEmploye { get; set; }
    }

    //enumeration
    public enum TypeEmploye
    {
        Veterinary,
        Asistent,
        Administive,
        Manteinance,
        Groomer
    }
}
