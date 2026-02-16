import { EHorarioComida } from '../../enum/EHorarioComida';

export interface DietaComidaRequestDTO {
    id_Comida: number;
    cantidad: string;
    nombreCategoria: string;
    es_Permitido: boolean;
    dia: number;
    momento: EHorarioComida;
}