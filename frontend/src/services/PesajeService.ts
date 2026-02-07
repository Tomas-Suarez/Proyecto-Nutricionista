import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { PesajeRequestDTO } from "../types/dto/request/PesajeRequestDTO";
import type { PesajeResponseDTO } from "../types/dto/response/PesajeResponseDTO";
import type { PagedResponse } from "../types/common/PagedResponse";

export const PesajeService = {
  async registrar(dto: PesajeRequestDTO): Promise<PesajeResponseDTO> {
    const response = await apiClient.post<PesajeResponseDTO>(
      ApiRoutes.Pesaje.Registrar,
      dto,
    );
    return response.data;
  },

  async obtenerMiHistorial(
    page: number = 1,
    size: number = 10,
  ): Promise<PagedResponse<PesajeResponseDTO>> {
    const response = await apiClient.get<PagedResponse<PesajeResponseDTO>>(
      ApiRoutes.Pesaje.ObtenerMiHistorial(page, size),
    );
    return response.data;
  },

  async obtenerPorId(idPesaje: number): Promise<PesajeResponseDTO> {
    const response = await apiClient.get<PesajeResponseDTO>(
      ApiRoutes.Pesaje.ObtenerPorId(idPesaje),
    );
    return response.data;
  },

  async eliminar(idPesaje: number): Promise<void> {
    await apiClient.delete<void>(ApiRoutes.Pesaje.Eliminar(idPesaje));
  },
};
