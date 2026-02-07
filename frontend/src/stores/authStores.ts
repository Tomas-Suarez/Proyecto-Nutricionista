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
    cargando.value = true;
    try {
      const data = await UsuarioService.login(credenciales);
      usuario.value = data;
      return usuario.value;
    } catch (error) {
      usuario.value = null;
      throw error;
    } finally {
      cargando.value = false;
    }
  }

  async function verificarSesion() {
    cargando.value = true;
    try {

      const data = await UsuarioService.refrescarToken();
      usuario.value = data; 
    } catch (error) {
      usuario.value = null;
    } finally {
      cargando.value = false;
    }
  }

  async function logout() {
    try {
      await UsuarioService.logout(); 
    } catch (error) {
      console.error("Error logout", error);
    } finally {
      usuario.value = null;
      router.push('/login');
    }
  }

  return {
    usuario,
    cargando,
    estaAutenticado,
    rolUsuario,
    login,
    verificarSesion,
    logout
  };
});