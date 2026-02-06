export interface PacienteRequest {
    Id_Usuario: number;
    Nombre: string;
    Apellido: string;
    Dni: string;
    Email: string;
    Telefono?: string;
    Genero: string;
    Altura_Cm: number;
}