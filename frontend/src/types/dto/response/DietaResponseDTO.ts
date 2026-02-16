import type { DietaComidaResponseDTO } from './DietaComidaResponseDTO';

export interface DietaResponseDTO {
    id_Dieta: number;
    nombre: string;
    descripcion: string;
    fecha_Inicio: string;
    fecha_Fin: string;
    activa: boolean;
    pacienteNombre: string;
    comidas: DietaComidaResponseDTO[];
}