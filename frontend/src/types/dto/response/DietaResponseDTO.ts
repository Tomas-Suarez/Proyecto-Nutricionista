import type { DietaComidaResponseDTO } from './DietaComidaResponseDTO';

export interface DietaResponseDTO {
    id_Dieta: number;
    nombre: string;
    pacienteNombre: string;
    totalCalorias: number;
    totalProteinas: number;
    totalCarbohidratos: number;
    totalGrasas: number;
    comidasDetalle?: DietaComidaResponseDTO[]; 
}