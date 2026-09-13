using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;


namespace Proyecto1_1_1548_0877.Models
{
    public class Procedures
    {
        [Key]
        [Display(Name = "ID Procedure")]
        public int Id { get; set; }



        // Customer relation
        [Required(ErrorMessage = "You have to select a customer")]
        [Display(Name = "Customer (Owner)")]
        [ForeignKey("Customer")]
        public int IdCustomer { get; set; }

        // Pets relation
        [Display(Name = "Pet")]
        [Required(ErrorMessage = "You have to select a pet")]
        [ForeignKey("Pets")]
        public int PetId { get; set; }


        public virtual Pets? Pets { get; set; }

        [Display(Name = "Procedure type")]
        [Required(ErrorMessage = "You have to select a procedure")]
        public ProcedureType ProcedureType { get; set; }

        [Display(Name = "Weight (kg)")]
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
        [Required(ErrorMessage = "You have to select a status")]
        public ProcedureStatus Status { get; set; }
    }

    public enum ProcedureType
    {
        [Display(Name = "Consultation")]
        Consultation = 1,

        [Display(Name = "Consultation during special hours")]
        ConsultationSpecialHours,

        [Display(Name = "Neutering (0–5 kg)")]
        Neutering_0_5,

        [Display(Name = "Neutering (5–10 kg)")]
        Neutering_5_10,

        [Display(Name = "Neutering (10–20 kg)")]
        Neutering_10_20kg,

        [Display(Name = "Neutering (20–30 kg)")]
        Neutering_20_30kg,

        [Display(Name = "Neutering (30–50 kg)")]
        Neutering_30_50kg,

        [Display(Name = "Minor surgery")]
        MinorSurgery,

        [Display(Name = "Surgery")]
        Surgery,

        [Display(Name = "Grooming small")]
        Grooming_Small,

        [Display(Name = "Grooming medium")]
        Grooming_Medium,

        [Display(Name = "Grooming big")]
        Grooming_Big,

        [Display(Name = "Grooming extra big")]
        Grooming_ExtraBig,

        [Display(Name = "Annual vaccines")]
        AnnualVaccines
    }

    public enum ProcedureStatus
    {
        InProgress,
        Invoiced,
        Booked,
    }
}