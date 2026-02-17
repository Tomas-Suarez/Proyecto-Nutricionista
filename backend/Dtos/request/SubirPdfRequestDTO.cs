using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record SubirPdfRequestDTO(
    
    [Required]
    IFormFile Archivo,
    
    string? NombrePersonalizado
);