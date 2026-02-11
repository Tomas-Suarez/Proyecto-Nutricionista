import apiClient from "./ApiClient"
import { ApiRoutes } from "../constants/ApiRoutes"
import type { PatologiaResponseDTO } from "../types/dto/response/PatologiaResponseDTO"
import type { PatologiaRequestDTO } from "../types/dto/request/PatologiaRequestDTO"

export const PatologiaService = {
    async listarTodas(): Promise<PatologiaResponseDTO[]> {
        const response = await apiClient.get<PatologiaResponseDTO[]>(ApiRoutes.Patologia.ObtenerTodas);
        return response.data;
    },

    async crear(dto: PatologiaRequestDTO): Promise<PatologiaResponseDTO> {
        const response = await apiClient.post<PatologiaResponseDTO>(ApiRoutes.Patologia.Crear, dto);
        return response.data;
    },

    async modificar(id: number, dto: PatologiaRequestDTO): Promise<PatologiaResponseDTO> {
        const response = await apiClient.put<PatologiaResponseDTO>(ApiRoutes.Patologia.Modificar(id), dto);
        return response.data;
    },

    async eliminar(id: number): Promise<void>{
        await apiClient.delete<void>(ApiRoutes.Patologia.Eliminar(id))
    }
}