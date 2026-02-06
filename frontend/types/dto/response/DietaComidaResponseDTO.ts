export interface DietaComidaResponseDTO {
    id_Dieta_Comida: number;
    id_Comida: number;
    nombreComida: string;
    cantidad: number;
    horario: string;
    nota?: string;
    caloriasProporcionales: number;
    proteinasProporcionales: number;
    carbohidratosProporcionales: number;
    grasasProporcionales: number;
}