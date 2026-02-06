import { ERol } from '../../enum/ERol'

export interface UsuarioRequestDTO{
    Email: string;
    Password: string;
    Rol: ERol;
}