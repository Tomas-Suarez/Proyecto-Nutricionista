using System.ComponentModel.DataAnnotations;
using backend.Enum;

namespace backend.Dtos.request;

public record DietaComidaRequestDTO(
[Required]
    int Id_Comida,

    [Required]
    string Cantidad,

    [Required]
    bool Es_Permitido,

    [Required]
    int Dia,

    [Required]
    EMomento Momento
);
