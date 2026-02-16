using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Enum;

namespace backend.Models;

[Table("Paciente")]
public class PacienteEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Paciente { get; set; }

    public int? Id_Nutricionista { get; set; }

    [ForeignKey("Id_Nutricionista")]
    public virtual NutricionistaEntity? Nutricionista { get; set; }

    public int? Id_Objetivo { get; set; } 

    [ForeignKey("Id_Objetivo")]
    public virtual ObjetivoEntity? Objetivo { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Dni { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(100)]
    public string? Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Genero { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Peso_Inicial { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Altura_Cm { get; set; }

    public EEstadoPaciente Estado { get; set; } = EEstadoPaciente.Activo;
    
    [StringLength(100)]
    public string? TokenAcceso { get; set; }

    [StringLength(10)]
    public string? CodigoAcceso { get; set; }
    public virtual ICollection<PatologiaPacienteEntity> PatologiaPacientes { get; set; } = new List<PatologiaPacienteEntity>();
    public virtual ICollection<PesajeEntity> Pesajes { get; set; } = new List<PesajeEntity>();
}