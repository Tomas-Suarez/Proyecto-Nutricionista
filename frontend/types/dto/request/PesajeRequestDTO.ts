export interface PesajeRequestDTO{
    Id_Paciente: number;
    Peso_Kg: number;
    Porcentaje_Grasa: number;
    Masa_Muscular_Kg: number;
    Fecha_Pesaje?: string;
    Nota?: string;
}