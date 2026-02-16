import type { DietaComidaRequestDTO } from './DietaComidaRequestDTO';

export interface DietaRequestDTO {
    id_Paciente: number;
    id_Nutricionista: number;
    nombre: string;
    descripcion: string;
    fecha_Inicio: string;
    fecha_Fin: string;
    activa: boolean;
    comidas: DietaComidaRequestDTO[];
}