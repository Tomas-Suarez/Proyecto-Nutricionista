export interface PacienteRequestDTO {
    Nombre: string;
    Apellido: string;
    Dni: string;
    Email: string;
    Telefono?: string;
    Genero: string;
    Altura_Cm: number;
    Peso_Inicial: number;
    Id_Objetivo?: number | null;
    IdsPatologias: number[];
}