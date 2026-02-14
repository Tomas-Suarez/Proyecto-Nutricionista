export interface PacienteResponseDTO {
    Id_Paciente: number;
    Nombre: string;
    Apellido: string;
    Dni: string;
    Email?: string;
    Telefono: string;
    Genero?: string;
    Peso_Inicial: number;
    Altura_Cm: number;
    Estado: string;

    TokenAcceso?: string;
    CodigoAcceso?: string;
    
    Objetivo?: { Id_Objetivo: number; Nombre: string }; 
    Patologias: { Id_Patologia: number; Nombre: string }[]; 
}