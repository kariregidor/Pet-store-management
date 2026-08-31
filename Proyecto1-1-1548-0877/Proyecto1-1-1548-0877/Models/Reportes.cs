using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Proyecto1_1_1548_0877.Models
{
    public class Reportes
    {
        [Key]
        public int IdReporte { get; set; }

        [Display(Name = "Procedimiento")]
        public int ProcedimientoId { get; set; }

        // Relación con cliente
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }
        public string? NombreCliente { get; set; }

        // Relación con mascota
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }
        public string? NombreMascota { get; set; }

        // Datos del procedimiento
        [Display(Name = "Tipo de Procedimiento")]
        public TipoProcedimiento? TipoProcedimiento { get; set; }

        [Display(Name = "Estado del Procedimiento")]
        public EstadoProcedimiento? EstadoProcedimiento { get; set; }

        [Display(Name = "Peso")]
        public double? Peso { get; set; }

        [Display(Name = "Precio Base")]
        public decimal? PrecioBase { get; set; }

        [Display(Name = "IVA")]
        public decimal? IVA { get; set; }

        [Display(Name = "Precio Total")]
        public decimal? PrecioTotal { get; set; }



        public DateTime? FechaVacunacion { get; set; }

        [JsonIgnore]
        public string FechaVacunacionFormateada => FechaVacunacion?.ToString("yyyy-MM-dd");

        public virtual Cliente? Cliente { get; set; }
        public virtual Mascotas? Mascota { get; set; }

    }
}