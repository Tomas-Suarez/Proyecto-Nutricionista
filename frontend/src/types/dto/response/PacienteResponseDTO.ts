export interface PacienteResponseDTO {
    Id_Paciente : number,
    Id_Usuario: number,
    NombreCompleto: string,
    Dni: string,
    Email: string,
    Telefono?: string,
    Altura_Cm: number,
    Estado: string
}