import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { NutricionistaResponseDTO } from "../types/dto/response/NutricionistaResponseDTO";
import type { RegistroNutricionistaDTO } from "../types/dto/request/RegistroNutricionistaDTO";
import type { NutricionistaRequestDTO } from "../types/dto/request/NutricionistaRequestDTO";
import type { SubirPdfRequestDTO } from "../types/dto/request/SubirPdfRequestDTO";
import type { ArchivoResponseDTO } from "../types/dto/response/ArchivoResponseDTO";

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

async subirPdf(dto: SubirPdfRequestDTO): Promise<ArchivoResponseDTO> {
    const formData = new FormData();
    
    formData.append("Archivo", dto.archivo); 
    
    if (dto.nombrePersonalizado) {
      formData.append("NombrePersonalizado", dto.nombrePersonalizado);
    }

    const response = await apiClient.post<ArchivoResponseDTO>(
      ApiRoutes.Nutricionista.SubirPdf,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data", 
        },
      }
    );
    return response.data;
  },

  async obtenerMisArchivos(): Promise<ArchivoResponseDTO[]> {
    const response = await apiClient.get<ArchivoResponseDTO[]>(
      ApiRoutes.Nutricionista.MisArchivos,
    );
    return response.data;
  },

  async eliminarArchivo(id: number): Promise<void> {
    await apiClient.delete(
      ApiRoutes.Nutricionista.EliminarArchivo(id)
    );
  },

};
