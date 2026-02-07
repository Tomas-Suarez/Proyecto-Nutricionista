import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { CategoriaResponseDTO } from "../types/dto/response/CategoriaResponseDTO";
import type { CategoriaRequestDTO } from "../types/dto/request/CategoriaRequestDTO";

export const CategoriaService = {
  async obtenerTodas(): Promise<CategoriaResponseDTO[]> {
    const response = await apiClient.get<CategoriaResponseDTO[]>(
      ApiRoutes.Categoria.ObtenerTodas,
    );
    return response.data;
  },

  async crear(dto: CategoriaRequestDTO): Promise<CategoriaResponseDTO> {
    const response = await apiClient.post<CategoriaResponseDTO>(
      ApiRoutes.Categoria.Crear,
      dto,
    );
    return response.data;
  },

  async eliminar(idCategoria: number): Promise<void> {
    await apiClient.delete<void>(ApiRoutes.Categoria.Eliminar(idCategoria));
  },

  async obtenerPorId(idCategoria: number): Promise<CategoriaResponseDTO> {
    const response = await apiClient.get<CategoriaResponseDTO>(
      ApiRoutes.Categoria.ObtenerPorId(idCategoria),
    );
    return response.data;
  },
};
