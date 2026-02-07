import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { UsuarioService } from '../services/UsuarioService';
import type { UsuarioRequestDTO } from '../types/dto/request/UsuarioRequestDTO';
import type { UsuarioResponseDTO } from '../types/dto/response/UsuarioResponseDTO';
import router from '../router';

export const useAuthStore = defineStore('auth', () => {
  const usuario = ref<UsuarioResponseDTO | null>(null);
  const cargando = ref(true);

  const estaAutenticado = computed(() => !!usuario.value);
  const rolUsuario = computed(() => usuario.value?.Rol);

async function login(credenciales: UsuarioRequestDTO) {
    try {
      const data = await UsuarioService.login(credenciales);
      
      const { token, refreshToken, ...datosUsuario } = data;
      
      usuario.value = datosUsuario as UsuarioResponseDTO; 
      
      return usuario.value;
    } catch (error) {
      limpiarEstado();
      throw error;
    }
}

  async function verificarSesion() {
    cargando.value = true;
    try {
      const data = await UsuarioService.refrescarToken();
      usuario.value = data;
    } catch (error) {
      limpiarEstado();
    } finally {
      cargando.value = false;
    }
  }

  async function logout() {
    try {
      await UsuarioService.logout(); 
    } catch (error) {
      console.error("Error al cerrar sesión en servidor", error);
    } finally {
      limpiarEstado();
      router.push('/login');
    }
  }

  function limpiarEstado() {
    usuario.value = null;
  }

  return {
    usuario,
    cargando,
    estaAutenticado,
    rolUsuario,
    login,
    verificarSesion,
    limpiarEstado,
    logout
  };
});