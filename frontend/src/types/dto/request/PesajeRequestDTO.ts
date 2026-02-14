export interface PesajeRequestDTO {
    Id_Paciente: number;
    Peso_Kg: number;
    Fecha_Pesaje?: Date | string;
    Porcentaje_Grasa?: number | null;
    Masa_Muscular_Kg?: number | null;
    Nota?: string;
}