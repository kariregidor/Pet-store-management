using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Proyecto2_1_1548_0877.Models
{
    public class Reports
    {
        [Key]
        public int IdReport { get; set; }

        [Display(Name = "Procedure")]
        public int Id { get; set; }

        // Relation with customer
        [Display(Name = "Customer")]
        public int IdCustomer { get; set; }
        public string? Name { get; set; }

        // Relation with pet
        [Display(Name = "Pet")]
        public int PetId { get; set; }
        public string? PetName { get; set; }

        // Procedure data
        [Display(Name = "Procedure Type")]
        public ProcedureType? ProcedureType { get; set; }

        [Display(Name = "Procedure Status")]
        public ProcedureStatus? ProcedureStatus { get; set; }

        [Display(Name = "Weight")]
        public double? Weight { get; set; }

        [Display(Name = "Base Price")]
        public decimal? BasePrice { get; set; }

        [Display(Name = "VAT")]
        public decimal? VAT { get; set; }

        [Display(Name = "Total Price")]
        public decimal? TotalPrice { get; set; }




        public DateTime? VaccinationDate { get; set; }

        [JsonIgnore]
        public string? FormattedVaccinationDate => VaccinationDate?.ToString("yyyy-MM-dd");

        public virtual Customer? Customer { get; set; }
        public virtual Pets? Pets { get; set; }


    }
}
