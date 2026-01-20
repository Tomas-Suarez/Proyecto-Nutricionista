using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record PesajeRequestDTO(
    [Required(ErrorMessage = "El ID del paciente es obligatorio.")]
    int Id_Paciente,

    [Required(ErrorMessage = "El peso es obligatorio.")]
    [Range(20, 500, ErrorMessage = "El peso debe estar entre 20 y 500 kg.")]
    decimal Peso_Kg,

    [Range(1, 70, ErrorMessage = "El porcentaje de grasa no es válido.")]
    decimal? Porcentaje_Grasa,

    [Range(10, 200, ErrorMessage = "La masa muscular no es válida.")]
    decimal? Masa_Muscular_Kg,

    DateTime? Fecha_Pesaje,

    [StringLength(500, ErrorMessage = "La nota no puede superar los 500 caracteres.")]
    string? Nota
);
