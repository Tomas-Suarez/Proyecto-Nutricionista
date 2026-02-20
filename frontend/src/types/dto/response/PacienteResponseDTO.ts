import type { ArchivoResponseDTO } from "./ArchivoResponseDTO";
import type { DietaResponseDTO } from "./DietaResponseDTO";
import type { ObjetivoResponseDTO } from "./ObjetivoResponseDTO";
import type { PatologiaResponseDTO } from "./PatologiaResponseDTO";
import type { PesajeResponseDTO } from "./PesajeResponseDTO";

export interface PacienteResponseDTO {
    Id_Paciente: number;
    Nombre: string;
    Apellido: string;
    Dni: string;
    Email: string;
    Telefono?: string;
    Genero?: string;
    Peso_Inicial: number;
    Altura_Cm: number;
    Estado: string;
    TokenAcceso?: string;
    CodigoAcceso?: string;
    
    PesoActual: number;
    Imc: number;
    
    Objetivo?: ObjetivoResponseDTO;
    Patologias: PatologiaResponseDTO[];

    ArchivosNutricionista: ArchivoResponseDTO[];
    HistorialPeso: PesajeResponseDTO[];
    DietaActual?: DietaResponseDTO;
}