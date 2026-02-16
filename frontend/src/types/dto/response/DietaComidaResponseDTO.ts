export interface DietaComidaResponseDTO {
    id_Dieta_Comida: number;
    id_Comida: number;
    nombreComida: string;
    cantidad: string;
    es_Permitido: boolean;
    dia: number;
    momento: string;
}