import axios from 'axios';
import { ApiRoutes } from '../constants/ApiRoutes';

import type { PacienteResponseDTO } from '../types/dto/response/PacienteResponseDTO';
import type { PesajeResponseDTO } from '../types/dto/response/PesajeResponseDTO';
import type { DietaResponseDTO } from '../types/dto/response/DietaResponseDTO';
import type { PagedResponse } from '../types/common/PagedResponse';

const publicApi = axios.create({
    baseURL: ApiRoutes.BASE_URL
});

export const PublicPacienteService = {
    async acceder(token: string, codigo: string): Promise<PacienteResponseDTO> {
        const response = await publicApi.post(ApiRoutes.Paciente.AccederPublico, { token, codigo });
        return response.data;
    },

    async getPesajes(token: string, codigo: string, page = 1, size = 10): Promise<PagedResponse<PesajeResponseDTO>> {
        const response = await publicApi.post(
            `${ApiRoutes.Pesaje.ObtenerHistorialPublico}?page=${page}&size=${size}`,
            { token, codigo }
        );
        return response.data;
    },

    async getDietaActiva(token: string, codigo: string): Promise<DietaResponseDTO> {
        const response = await publicApi.post(ApiRoutes.Dieta.ObtenerDietaPublica, { token, codigo });
        return response.data;
    }
};