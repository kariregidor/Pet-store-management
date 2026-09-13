using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Proyecto1_1_1548_0877.Models
{
    public class Reports
    {
        [Key]
        public int IdReport { get; set; }

        [Display(Name = "Procedure")]
        public int Id { get; set; }

        // Customer relation
        [Display(Name = "Customer")]
        public int IdCustomer { get; set; }
        public string? Name { get; set; }

        // RPet relation
        [Display(Name = "Pet")]
        public int PetId { get; set; }
        public string? PetName { get; set; }

        // Procedure data
        [Display(Name = "Procedure type")]
        public ProcedureType? ProcedureType { get; set; }

        [Display(Name = "Procedure status")]
        public ProcedureStatus ProcedureStatus { get; set; }

        [Display(Name = "Weigth")]
        public double? Weight { get; set; }

        [Display(Name = "Base price")]
        public decimal? BasePrice { get; set; }

        [Display(Name = "VAT")]
        public decimal? VAT { get; set; }

        [Display(Name = "Total price")]
        public decimal? TotalPrice { get; set; }



        public DateTime? VaccinationDate { get; set; }

        [JsonIgnore]
        public string? FormattedVaccinationDate => VaccinationDate?.ToString("yyyy-MM-dd");

        public virtual Customer? Customer { get; set; }
        public virtual Pets? Pets { get; set; }

    }
}