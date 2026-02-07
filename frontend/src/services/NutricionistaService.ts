import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { NutricionistaResponseDTO } from "../types/dto/response/NutricionistaResponseDTO";
import type { RegistroNutricionistaDTO } from "../types/dto/request/RegistroNutricionistaDTO";
import type { NutricionistaRequestDTO } from "../types/dto/request/NutricionistaRequestDTO";

export const NutricionistaService = {
  async registrar(
    dto: RegistroNutricionistaDTO,
  ): Promise<NutricionistaResponseDTO> {
    const response = await apiClient.post<NutricionistaResponseDTO>(
      ApiRoutes.Nutricionista.Registrar,
      dto,
    );
    return response.data;
  },

  async obtenerMiPerfil(): Promise<NutricionistaResponseDTO> {
    const response = await apiClient.get<NutricionistaResponseDTO>(
      ApiRoutes.Nutricionista.ObtenerMiPerfil,
    );
    return response.data;
  },

  async modificarMiPerfil(
    dto: NutricionistaRequestDTO,
  ): Promise<NutricionistaResponseDTO> {
    const response = await apiClient.put<NutricionistaResponseDTO>(
      ApiRoutes.Nutricionista.ModificarMiPerfil,
      dto,
    );
    return response.data;
  },
};
