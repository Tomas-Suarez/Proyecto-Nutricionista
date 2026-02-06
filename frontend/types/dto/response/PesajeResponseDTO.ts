// src/types/dto/pesaje/PesajeResponseDTO.ts

export interface PesajeResponseDTO {
    id_Pesaje: number;
    id_Paciente: number;
    peso_Kg: number;
    porcentaje_Grasa?: number;
    masa_Muscular_Kg?: number;
    fecha_Pesaje: string;
    nota?: string;
    
    imc: number;
    clasificacionImc: string;
    diferenciaPesoAnterior: number;
}