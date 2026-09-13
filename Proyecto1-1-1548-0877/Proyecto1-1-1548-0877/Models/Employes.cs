using System.ComponentModel.DataAnnotations;

namespace Proyecto1_1_1548_0877.Models
{
    public class Employes
    {
        [Key]
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is mandatory")]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of birth is mandatory")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Start date is mandatory")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Salary per day")]
        [Required(ErrorMessage = "This field is mandatory")]
        public decimal SalaryPerDay { get; set; }

        [Display(Name = "Retirement date")]
        [DataType(DataType.Date)]
        public DateTime? RetirementDate { get; set; }

        [Display(Name = "Type employe")]
        [Required(ErrorMessage = "Type employe is mandatory")]
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

