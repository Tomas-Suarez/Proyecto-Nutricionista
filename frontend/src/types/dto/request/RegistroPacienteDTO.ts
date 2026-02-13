export interface RegistroPacienteDTO{
    Email: string;
    Nombre: string;
    Apellido: string;
    Dni: string;
    Telefono: string;
    Genero: string;
    Peso_Inicial: number;
    Altura_Cm: number;
    IdObjetivo?: number | null;
    IdsPatologias?: number[];
}