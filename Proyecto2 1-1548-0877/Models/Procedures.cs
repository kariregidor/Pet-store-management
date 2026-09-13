using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto2_1_1548_0877.Models
{
    public class Procedures
    {
        [Key]
        [Display(Name = "Procedure ID")]
        public int Id { get; set; }



        // Relation with Customer - uses IdCustomer as Foreign Key
        [Required(ErrorMessage = "You must select a customer")]
        [Display(Name = "Customer (Owner)")]
        [ForeignKey("Customer")]
        public int IdCustomer { get; set; }

        [Display(Name = "Pet")]
        [Required(ErrorMessage = "You must select the pet")]
        [ForeignKey("Pets")]
        public int PetId { get; set; }

        public virtual Pets? Pets { get; set; }


        [Display(Name = "Procedure type")]
        [Required(ErrorMessage = "You must select the procedure type")]
        public ProcedureType ProcedureType { get; set; }

        [Display(Name = "Patient weight (kg)")]
        public double? Weight { get; set; }

        [Display(Name = "Base price")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:₡#,##0}", ApplyFormatInEditMode = false)]
        public decimal BasePrice { get; set; }

        [Display(Name = "VAT (13%)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:₡#,##0}", ApplyFormatInEditMode = false)]
        public decimal VAT => BasePrice * 0.13m;

        [Display(Name = "Total price with VAT")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:₡#,##0}", ApplyFormatInEditMode = false)]
        public decimal TotalPrice => BasePrice + VAT;

        [Display(Name = "Procedure status")]
        [Required(ErrorMessage = "You must select a status for the procedure")]
        public ProcedureStatus Status { get; set; }
    }

    public enum ProcedureType
    {
        [Display(Name = "Consultation")]
        Consultation = 1,

        [Display(Name = "Consultation during special hours")]
        ConsultationSpecialHours,

        [Display(Name = "Neutering 0-5kg")]
        Neutering_0_5,

        [Display(Name = "Neutering 5-10kg")]
        Neutering_5_10,

        [Display(Name = "Neutering 10-20kg")]
        Neutering_10_20kg,

        [Display(Name = "Neutering 20-30kg")]
        Neutering_20_30kg,

        [Display(Name = "Neutering 30-50kg")]
        Neutering_30_50kg,

        [Display(Name = "Minor surgery")]
        MinorSurgery,

        [Display(Name = "Major surgery")]
        Surgery,

        [Display(Name = "Small grooming")]
        Grooming_Small,

        [Display(Name = "Medium grooming")]
        Grooming_Medium,

        [Display(Name = "Large grooming")]
        Grooming_Big,

        [Display(Name = "Extra large grooming")]
        Grooming_ExtraBig,

        [Display(Name = "Annual vaccines")]
        AnnualVaccines
    }

    public enum ProcedureStatus
    {
        InProgress,
        Invoiced,
        Booked
    }
}
