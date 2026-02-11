import apiClient from "./ApiClient"
import { ApiRoutes } from "../constants/ApiRoutes"
import type { ObjetivoRequestDTO } from "../types/dto/request/ObjetivoRequestDTO";
import type { ObjetivoResponseDTO } from "../types/dto/response/ObjetivoResponseDTO";

export const ObjetivoService = {
    async listarTodas(): Promise<ObjetivoResponseDTO[]> {
        const response = await apiClient.get<ObjetivoResponseDTO[]>(ApiRoutes.Objetivo.ObtenerTodas);
        return response.data;
    },

    async crear(dto: ObjetivoRequestDTO): Promise<ObjetivoResponseDTO> {
        const response = await apiClient.post<ObjetivoResponseDTO>(ApiRoutes.Objetivo.Crear, dto);
        return response.data;
    },

    async modificar(id: number, dto: ObjetivoRequestDTO): Promise<ObjetivoResponseDTO> {
        const response = await apiClient.put<ObjetivoResponseDTO>(ApiRoutes.Objetivo.Modificar(id), dto);
        return response.data;
    },

    async eliminar(id: number): Promise<void>{
        await apiClient.delete<void>(ApiRoutes.Objetivo.Eliminar(id))
    }
}