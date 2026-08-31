using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;


namespace Proyecto1_1_1548_0877.Models
{
    public class Procedimientos
    {
        [Key]
        [Display(Name = "ID Procedimiento")]
        public int Id { get; set; }

     

        // Relación con Cliente 
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente (Dueño)")]
        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        // Relación con Mascota
        [Display(Name = "Mascotas")]
        [Required(ErrorMessage = "Debe seleccionar la mascota")]
        [ForeignKey("Mascota")]
        public int MascotaId { get; set; }


        public virtual Mascotas? Mascota { get; set; }  

        [Display(Name = "Tipo de procedimiento")]
        [Required(ErrorMessage = "Debe seleccionar el tipo de procedimiento")]
        public TipoProcedimiento TipoProcedimiento { get; set; }

        [Display(Name = "Peso del paciente (kg)")]
        public double? Peso { get; set; }   

        [Display(Name = "Precio base")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:₡#,##0}", ApplyFormatInEditMode = false)]
        public decimal PrecioBase { get; set; }

        [Display(Name = "IVA (13%)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:₡#,##0}", ApplyFormatInEditMode = false)]
        public decimal IVA => PrecioBase * 0.13m;

        [Display(Name = "Precio total con IVA")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:₡#,##0}", ApplyFormatInEditMode = false)]
        public decimal PrecioTotal => PrecioBase + IVA;

        [Display(Name = "Estado del procedimiento")]
        [Required(ErrorMessage = "Debe seleccionar un estado para el procedimiento")]
        public EstadoProcedimiento Estado { get; set; }
    }

    public enum TipoProcedimiento
    {
        [Display(Name = "Consulta")]
        Consulta = 1,

        [Display(Name = "Consulta en horario especial")]
        ConsultaHorarioEspecial,

        [Display(Name = "Castración 0-5kg")]
        Castracion_0_5kg,

        [Display(Name = "Castración 5-10kg")]
        Castracion_5_10kg,

        [Display(Name = "Castración 10-20kg")]
        Castracion_10_20kg,

        [Display(Name = "Castración 20-30kg")]
        Castracion_20_30kg,

        [Display(Name = "Castración 30-50kg")]
        Castracion_30_50kg,

        [Display(Name = "Cirugía menor")]
        CirugiaMenor,

        [Display(Name = "Cirugía mayor")]
        CirugiaMayor,

        [Display(Name = "Grooming pequeña")]
        Grooming_Pequeña,

        [Display(Name = "Grooming mediana")]
        Grooming_Mediana,

        [Display(Name = "Grooming grande")]
        Grooming_Grande,

        [Display(Name = "Grooming extra grande")]
        Grooming_ExtraGrande,

        [Display(Name = "Vacunas anuales")]
        VacunasAnuales
    }

    public enum EstadoProcedimiento
    {
        EnProceso,
        Facturado,
        Agendado
    }
}