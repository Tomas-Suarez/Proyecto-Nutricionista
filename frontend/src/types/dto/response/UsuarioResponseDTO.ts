import { ERol } from '../../enum/ERol'

export interface UsuarioResponseDTO{
    Id_Usuario: number;
    Email: string;
    Rol: ERol;
    AvatarUrl?: string
}