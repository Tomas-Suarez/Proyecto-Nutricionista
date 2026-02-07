import type { UsuarioResponseDTO } from './UsuarioResponseDTO';

export interface LoginResponseDTO extends UsuarioResponseDTO{
    token: string;
    refreshToken: string;
}