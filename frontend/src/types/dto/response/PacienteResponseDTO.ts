import type { ArchivoResponseDTO } from "./ArchivoResponseDTO";
import type { DietaResponseDTO } from "./DietaResponseDTO";
import type { ObjetivoResponseDTO } from "./ObjetivoResponseDTO";
import type { PatologiaResponseDTO } from "./PatologiaResponseDTO";
import type { PesajeResponseDTO } from "./PesajeResponseDTO";

export interface PacienteResponseDTO {
    id_Paciente: number;
    nombre: string;
    apellido: string;
    dni: string;
    email: string;
    telefono?: string;
    genero?: string;
    peso_Inicial: number;
    altura_Cm: number;
    estado: string;
    tokenAcceso?: string;
    codigoAcceso?: string;
    
    pesoActual: number;
    imc: number;
    
    objetivo?: ObjetivoResponseDTO;
    patologias: PatologiaResponseDTO[];

    ArchivosNutricionista: ArchivoResponseDTO[];
    HistorialPeso: PesajeResponseDTO[];
    DietaActual?: DietaResponseDTO;
}