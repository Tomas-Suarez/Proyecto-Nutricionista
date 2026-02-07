import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { PagedResponse } from "../types/common/PagedResponse";
import type { ComidaResponseDTO } from "../types/dto/response/ComidaResponseDTO";
import type { ComidaRequestDTO } from "../types/dto/request/ComidaRequestDTO";

export const ComidaService = {
  async crear(dto: ComidaRequestDTO): Promise<ComidaResponseDTO> {
    const response = await apiClient.post<ComidaResponseDTO>(
      ApiRoutes.Comida.Crear,
      dto,
    );
    return response.data;
  },

  async obtenerTodas(
    page: number = 1,
    size: number = 10,
    idCategoria?: number,
    nombre?: string,
  ): Promise<PagedResponse<ComidaResponseDTO>> {
    const response = await apiClient.get<PagedResponse<ComidaResponseDTO>>(
      ApiRoutes.Comida.Listar(idCategoria, nombre, page, size),
    );
    return response.data;
  },

  async obtenerPorId(idComida: number): Promise<ComidaResponseDTO> {
    const response = await apiClient.get<ComidaResponseDTO>(
      ApiRoutes.Comida.ObtenerPorId(idComida),
    );
    return response.data;
  },

  async eliminar(idComida: number): Promise<void> {
    await apiClient.delete<void>(ApiRoutes.Comida.Eliminar(idComida));
  },

  async modificar(
    idComida: number,
    dto: ComidaRequestDTO,
  ): Promise<ComidaResponseDTO> {
    const response = await apiClient.put<ComidaResponseDTO>(
      ApiRoutes.Comida.Modificar(idComida),
      dto,
    );
    return response.data;
  },
};
