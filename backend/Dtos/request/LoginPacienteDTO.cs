namespace backend.Dtos.request;

public record LoginPacienteDTO(
    string Token,
    string Codigo
);