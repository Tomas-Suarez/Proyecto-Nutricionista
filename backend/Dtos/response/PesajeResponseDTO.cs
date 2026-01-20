using System.Text.Json.Serialization;
using backend.Enum;

namespace backend.Dtos.response;

public record PesajeResponseDTO(
    int Id_Pesaje,
    int Id_Paciente,
    decimal Peso_Kg,
    decimal? Porcentaje_Grasa,
    decimal? Masa_Muscular_Kg,
    DateTime Fecha_Pesaje,
    string? Nota,
    
    decimal Imc, 
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    EIMC ClasificacionImc,
    decimal DiferenciaPesoAnterior
);
