import { defineStore } from "pinia";
import { ref, computed } from "vue";
import { UsuarioService } from "../services/UsuarioService";
import { NutricionistaService } from "../services/NutricionistaService";
import type { UsuarioRequestDTO } from "../types/dto/request/UsuarioRequestDTO";
import type { UsuarioResponseDTO } from "../types/dto/response/UsuarioResponseDTO";
import router from "../router";

export const useAuthStore = defineStore("auth", () => {
  const usuario = ref<UsuarioResponseDTO | null>(null);
  const cargando = ref(true);

  const perfilStorage = localStorage.getItem("nutri_perfil");
  const perfil = ref<any>(perfilStorage ? JSON.parse(perfilStorage) : null);

  const estaAutenticado = computed(() => !!usuario.value);
  const rolUsuario = computed(() => usuario.value?.Rol);

  const nombreMostrar = computed(() => {
    if (perfil.value?.Nombre) return `${perfil.value.Nombre}`;
    return usuario.value?.Email || "Usuario";
  });

  const iniciales = computed(() => {
    if (perfil.value?.Nombre)
      return perfil.value.Nombre.charAt(0).toUpperCase();
    if (usuario.value?.Email)
      return usuario.value.Email.charAt(0).toUpperCase();
    return "U";
  });

  async function login(credenciales: UsuarioRequestDTO) {
    cargando.value = true;
    try {
      const data = await UsuarioService.login(credenciales);
      usuario.value = data;

      if (data.Rol.toString() === "Nutricionista") {
        try {
          const perfilData = await NutricionistaService.obtenerMiPerfil();
          perfil.value = perfilData;
          localStorage.setItem("nutri_perfil", JSON.stringify(perfilData));
        } catch (e) {
          console.warn(
            "Login exitoso, pero no se pudo cargar el perfil detallado",
            e,
          );
        }
      } else {
        perfil.value = null;
        localStorage.removeItem("nutri_perfil");
      }

      return usuario.value;
    } catch (error) {
      usuario.value = null;
      perfil.value = null;
      localStorage.removeItem("nutri_perfil");
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

      if (
        usuario.value &&
        usuario.value.Rol.toString() === "Nutricionista" &&
        !perfil.value
      ) {
        const storageData = localStorage.getItem("nutri_perfil");
        if (storageData) {
          perfil.value = JSON.parse(storageData);
        } else {
          try {
            const perfilData = await NutricionistaService.obtenerMiPerfil();
            perfil.value = perfilData;
            localStorage.setItem("nutri_perfil", JSON.stringify(perfilData));
          } catch (e) {
            console.warn("No se pudo refrescar el perfil de nutricionista", e);
          }
        }
      }
    } catch (error) {
      usuario.value = null;
      logout();
    } finally {
      cargando.value = false;
    }
  }

  function actualizarPerfilLocal(nuevosDatos: any) {
    if (perfil.value) {
      const perfilActualizado = { ...perfil.value, ...nuevosDatos };
      perfil.value = perfilActualizado;
      localStorage.setItem("nutri_perfil", JSON.stringify(perfilActualizado));
    }

    if (usuario.value && nuevosDatos.AvatarUrl !== undefined) {
      usuario.value = {
        ...usuario.value,
        AvatarUrl: nuevosDatos.AvatarUrl,
      };
    }
  }

  async function logout() {
    try {
      await UsuarioService.logout();
    } catch (error) {
      console.error("Error logout", error);
    } finally {
      usuario.value = null;
      perfil.value = null;
      localStorage.removeItem("nutri_perfil");

      router.push("/login");
    }
  }

  return {
    usuario,
    perfil,
    cargando,
    estaAutenticado,
    rolUsuario,
    nombreMostrar,
    iniciales,
    login,
    verificarSesion,
    logout,
    actualizarPerfilLocal,
  };
});
