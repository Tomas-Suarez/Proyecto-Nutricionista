import type { UsuarioResponseDTO } from './UsuarioResponseDTO';

export interface LoginResponseDTO {
    usuario: UsuarioResponseDTO;
    token: string;
    refreshToken: string;
}