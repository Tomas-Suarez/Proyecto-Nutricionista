import { EHorarioComida } from '../../enum/EHorarioComida'

export interface DietaComidaRequestDTO {
    id_Comida: number;
    cantidad: number;
    horario: EHorarioComida;
    nota?: string;
}