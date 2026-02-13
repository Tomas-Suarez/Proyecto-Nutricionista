export interface PacienteResponseDTO {
    Id_Paciente: number;
    Id_Usuario: number;
    Nombre: string;
    Apellido: string;
    Dni: string;
    Email: string;
    AvatarUrl?: string;
    Telefono?: string;
    Genero?: string;
    Peso_Inicial: number;
    Altura_Cm: number;
    Estado: string;
    
    Objetivo?: { Id_Objetivo: number; Nombre: string }; 
    Patologias: { Id_Patologia: number; Nombre: string }[]; 
}