import apiClient from "./ApiClient";
import type { UsuarioRequestDTO } from "../types/dto/request/UsuarioRequestDTO";
import type { UsuarioResponseDTO } from "../types/dto/response/UsuarioResponseDTO";
import type { LoginResponseDTO } from "../types/dto/response/LoginResponseDTO";
import type { CambiarPasswordRequestDTO } from "../types/dto/request/CambiarPasswordRequestDTO";
import { ApiRoutes } from "../constants/ApiRoutes";

export const UsuarioService = {
  async registrar(dto: UsuarioRequestDTO): Promise<UsuarioResponseDTO> {
    const response = await apiClient.post(ApiRoutes.Usuario.Registrar, dto);
    return response.data;
  },

  async login(dto: UsuarioRequestDTO): Promise<LoginResponseDTO> {
    const response = await apiClient.post(ApiRoutes.Usuario.Login, dto);
    return response.data;
  },

  async refrescarToken(): Promise<LoginResponseDTO> {
    const response = await apiClient.post(ApiRoutes.Usuario.Refresh);
    return response.data;
  },

  async cambiarMiPassword(
    dto: CambiarPasswordRequestDTO,
  ): Promise<{ mensaje: string }> {
    const response = await apiClient.patch<{ mensaje: string }>(
      ApiRoutes.Usuario.CambiarPassword,
      dto,
    );
    return response.data;
  },

  async obtenerUsuarioPorId(idUsuario: number): Promise<UsuarioResponseDTO> {
    const response = await apiClient.get<UsuarioResponseDTO>(
      ApiRoutes.Usuario.ObtenerPorId(idUsuario),
    );
    return response.data;
  },

  async logout(): Promise<void> {
    await apiClient.post(ApiRoutes.Usuario.Logout);
  },

  async subirAvatar(archivo: File): Promise<{ url: string }> {
    const formData = new FormData();
    formData.append("archivo", archivo);

    const response = await apiClient.post<{ url: string }>(
      ApiRoutes.Usuario.SubirAvatar,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      },
    );
    return response.data;
  },

  async borrarAvatar(): Promise<{ mensaje: string; url: string }> {
    const response = await apiClient.delete<{ mensaje: string; url: string }>(
      ApiRoutes.Usuario.BorrarAvatar,
    );
    return response.data;
  },
};
