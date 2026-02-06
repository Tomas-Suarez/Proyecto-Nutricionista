import apiClient from "./ApiClient";
import { ApiRoutes } from "../constants/ApiRoutes";
import type { PacienteResponseDTO } from "../types/dto/response/PacienteResponseDTO";
import type { RegistroPacienteDTO } from "../types/dto/request/RegistroPacienteDTO";
import type { PacienteRequestDTO } from "../types/dto/request/PacienteRequestDTO";
import type { PagedResponse } from "../types/common/PagedResponse";
import { EEstadoPaciente } from "../types/enum/EEstadoPaciente";

export const PacienteService = {
  async registrar(dto: RegistroPacienteDTO): Promise<PacienteResponseDTO> {
    const response = await apiClient.post<PacienteResponseDTO>(
      ApiRoutes.Paciente.Registrar,
      dto,
    );
    return response.data;
  },

  async obtenerMiPerfil(): Promise<PacienteResponseDTO> {
    const response = await apiClient.get<PacienteResponseDTO>(
      ApiRoutes.Paciente.ObtenerMiPerfil,
    );
    return response.data;
  },

  async modificarMiPerfil(
    dto: PacienteRequestDTO,
  ): Promise<PacienteResponseDTO> {
    const response = await apiClient.put<PacienteResponseDTO>(
      ApiRoutes.Paciente.ModificarMiPerfil,
      dto,
    );
    return response.data;
  },

  async modificarPaciente(
    id: number,
    dto: PacienteRequestDTO,
  ): Promise<PacienteResponseDTO> {
    const response = await apiClient.put<PacienteResponseDTO>(
      ApiRoutes.Paciente.ActualizarPaciente(id),
      dto,
    );
    return response.data;
  },

  async obtenerPacientesPorNutricionista(
    page: number = 1,
    size: number = 10,
    estado?: EEstadoPaciente,
  ): Promise<PagedResponse<PacienteResponseDTO>> {
    const url = ApiRoutes.Paciente.Listar(estado, page, size);
    const response =
      await apiClient.get<PagedResponse<PacienteResponseDTO>>(url);

    return response.data;
  },

  async cambiarEstado(
    idPaciente: number,
    nuevoEstado: EEstadoPaciente,
  ): Promise<void> {
    const url = ApiRoutes.Paciente.CambiarEstado(idPaciente);
    await apiClient.patch(url, { nuevoEstado });
  },
};
