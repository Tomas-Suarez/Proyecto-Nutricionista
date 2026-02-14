import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { DietaResponseDTO } from "../types/dto/response/DietaResponseDTO";
import type { DietaRequestDTO } from "../types/dto/request/DietaRequestDTO";

export const DietaService = {
  async registrar(dto: DietaRequestDTO): Promise<DietaResponseDTO> {
    const response = await apiClient.post<DietaResponseDTO>(
      ApiRoutes.Dieta.Registrar,
      dto,
    );
    return response.data;
  },

  async modificarDieta(
    idDieta: number,
    dto: DietaRequestDTO,
  ): Promise<DietaResponseDTO> {
    const response = await apiClient.put<DietaResponseDTO>(
      ApiRoutes.Dieta.Modificar(idDieta),
      dto,
    );
    return response.data;
  },

  async eliminar(idDieta: number): Promise<void> {
    await apiClient.delete<void>(ApiRoutes.Dieta.Eliminar(idDieta));
  },

  async obtenerPorId(idDieta: number): Promise<DietaResponseDTO> {
    const response = await apiClient.get<DietaResponseDTO>(
      ApiRoutes.Dieta.ObtenerPorId(idDieta),
    );
    return response.data;
  },

  async obtenerHistorialPaciente(
    idPaciente: number,
  ): Promise<DietaResponseDTO[]> {
    const response = await apiClient.get<DietaResponseDTO[]>(
      ApiRoutes.Dieta.ObtenerHistorial(idPaciente),
    );
    return response.data;
  },

  async obtenerDietaActiva(idPaciente: number): Promise<DietaResponseDTO> {
    const response = await apiClient.get<DietaResponseDTO>(
      ApiRoutes.Dieta.ObtenerDietaActiva(idPaciente),
    );
    return response.data;
  },

  async activarDieta(idDieta: number): Promise<void> {
    await apiClient.patch<void>(
      ApiRoutes.Dieta.ActivarDieta(idDieta)
    );
  },
};
