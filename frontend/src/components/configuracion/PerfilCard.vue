<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { NutricionistaService } from '../../services/NutricionistaService';
import { UsuarioService } from '../../services/UsuarioService';
import { useConfirm } from "primevue/useconfirm";
import { useAuthStore } from '../../stores/authStores';
import { useToast } from "primevue/usetoast";

const confirm = useConfirm();
const authStore = useAuthStore();
const toast = useToast();

const cargando = ref(false);
const subiendoFoto = ref(false);
const fileInput = ref<HTMLInputElement | null>(null);

const esAdmin = computed(() => authStore.rolUsuario?.toString() === 'Admin');

const emailMostrar = ref(authStore.usuario?.Email || '');

const form = ref({
    Nombre: '',
    Apellido: '',
    Matricula: '',
    Telefono: ''
});

const avatarUrl = computed(() => {
    const url = authStore.usuario?.AvatarUrl;

    if (!url) return '';

    if (url.startsWith('http')) return url;

    const ulrImg = `${import.meta.env.VITE_IMG_BASE_URL}${url}`;
    return ulrImg;
});

const esImagenDefault = computed(() => {
    if (!avatarUrl.value) return true;
    return avatarUrl.value.includes('default-user') || avatarUrl.value.includes('gstatic.com');
});

onMounted(async () => {
    if (esAdmin.value) {
        if (authStore.usuario?.Email) {
            emailMostrar.value = authStore.usuario.Email;
        }
        return;
    }

    cargando.value = true;
    try {
        const perfilNutri = await NutricionistaService.obtenerMiPerfil();

        Object.assign(form.value, perfilNutri);

        if (authStore.usuario?.Email) {
            emailMostrar.value = authStore.usuario.Email;
        }

    } catch (error) {
        console.error("Error al cargar perfil:", error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar la información', life: 3000 });
    } finally {
        cargando.value = false;
    }
});

const abrirSelector = () => {
    fileInput.value?.click();
};

const alSeleccionarArchivo = async (event: Event) => {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
        return;
    }

    const archivo = input.files[0];

    if (archivo?.size! > 2 * 1024 * 1024) {
        toast.add({ severity: 'warn', summary: 'Archivo muy grande', detail: 'Máximo 2MB', life: 3000 });
        input.value = '';
        return;
    }

    if (!archivo!.type.startsWith('image/')) {
        toast.add({ severity: 'error', summary: 'Formato incorrecto', detail: 'Solo se permiten imágenes (JPG, PNG, WEBP)', life: 3000 });
        input.value = '';
        return;
    }

    subiendoFoto.value = true;

    try {
        const res = await UsuarioService.subirAvatar(archivo!);

        authStore.actualizarPerfilLocal({ AvatarUrl: res.url });

        toast.add({ severity: 'success', summary: 'Éxito', detail: 'Foto de perfil actualizada', life: 3000 });

    } catch (error) {
        console.error('Error al subir avatar:', error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo subir la imagen', life: 3000 });
    } finally {
        subiendoFoto.value = false;
        input.value = '';
    }
};

const borrarFoto = () => {
    confirm.require({
        message: '¿Volver a la foto predeterminada?',
        header: 'Confirmar',
        icon: 'pi pi-trash',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                const res = await UsuarioService.borrarAvatar();

                authStore.actualizarPerfilLocal({ AvatarUrl: res.url });

                toast.add({ severity: 'info', summary: 'Restaurado', detail: 'Avatar restaurado', life: 3000 });
            } catch (error) {
                console.error(error);
                toast.add({ severity: 'error', summary: 'Error', detail: 'Error al eliminar', life: 3000 });
            }
        }
    });
};

const guardar = async () => {
    if (esAdmin.value) return;

    confirm.require({
        message: '¿Guardar los cambios en tus datos?',
        header: 'Confirmación',
        icon: 'pi pi-check-circle',
        acceptLabel: 'Sí, guardar',
        rejectLabel: 'Cancelar',
        accept: async () => {
            await ejecutarGuardado();
        }
    });
};

const ejecutarGuardado = async () => {
    cargando.value = true;
    try {
        await NutricionistaService.modificarMiPerfil(form.value);

        authStore.actualizarPerfilLocal(form.value);

        toast.add({ severity: 'success', summary: 'Guardado', detail: 'Datos actualizados', life: 3000 });
    } catch (error) {
        console.error(error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron guardar los cambios', life: 3000 });
    } finally {
        cargando.value = false;
    }
};
</script>

<template>
    <div class="card border-0 shadow-sm rounded-4 h-100">
        <div class="card-body p-4">

            <div v-if="cargando" class="text-center py-5">
                <i class="pi pi-spin pi-spinner text-primary" style="font-size: 2rem"></i>
            </div>

            <div v-else>
                <h5 class="card-title fw-bold mb-3 text-primary">
                    <i class="pi pi-user me-2"></i>Mi Perfil
                    <span v-if="esAdmin" class="badge bg-secondary ms-2 text-white"
                        style="font-size: 0.8rem">Admin</span>
                </h5>

                <div class="d-flex flex-column align-items-center mb-4">
                    <div class="position-relative avatar-container shadow-sm">

                        <img v-if="avatarUrl" :src="avatarUrl" class="avatar-img"
                            :class="{ 'opacity-50': subiendoFoto }" alt="Avatar">

                        <div v-else class="avatar-circle">
                            {{ form.Nombre ? form.Nombre.charAt(0).toUpperCase() : (emailMostrar.charAt(0).toUpperCase()
                            || 'U') }}
                        </div>

                        <div class="avatar-overlay" @click="abrirSelector">
                            <i v-if="!subiendoFoto" class="pi pi-camera fs-2 text-white"></i>
                            <i v-else class="pi pi-spin pi-spinner fs-2 text-white"></i>
                        </div>

                        <button v-if="!esImagenDefault" @click.stop="borrarFoto"
                            class="btn btn-danger btn-sm btn-delete shadow" title="Eliminar foto" type="button">
                            <i class="pi pi-trash" style="font-size: 0.8rem"></i>
                        </button>

                        <input type="file" ref="fileInput" class="d-none"
                            accept="image/png, image/jpeg, image/jpg, image/webp" @change="alSeleccionarArchivo">
                    </div>
                    <div class="mt-2 small texto-ayuda">Haz clic en la foto para cambiarla</div>
                </div>

                <form @submit.prevent="guardar">
                    <div class="mb-3">
                        <label class="form-label">Correo Electrónico</label>
                        <input type="email" class="form-control" v-model="emailMostrar" readonly disabled>
                    </div>

                    <div v-if="!esAdmin">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Nombre</label>
                                <input type="text" class="form-control" v-model="form.Nombre" required>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Apellido</label>
                                <input type="text" class="form-control" v-model="form.Apellido" required>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Teléfono</label>
                                <input type="text" class="form-control" v-model="form.Telefono"
                                    placeholder="Ej: +54 9 11...">
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Matrícula</label>
                                <input type="text" class="form-control" v-model="form.Matricula"
                                    placeholder="Ej: MN-1234">
                            </div>
                        </div>

                        <button type="submit" class="btn btn-primary w-100 rounded-3 mt-2" :disabled="cargando">
                            {{ cargando ? 'Guardando...' : 'Guardar Cambios' }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<style scoped>
.avatar-container {
    width: 110px;
    height: 110px;
    position: relative;
    border-radius: 50%;
    cursor: pointer;
    border: 3px solid white;
    outline: 2px solid var(--primary-color, #695CFE);
}

.avatar-img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 50%;
}

.avatar-circle {
    width: 100%;
    height: 100%;
    background-color: var(--primary-color, #695CFE);
    color: white;
    font-size: 2.5rem;
    font-weight: bold;
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 50%;
}

.avatar-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    border-radius: 50%;
    display: flex;
    justify-content: center;
    align-items: center;
    opacity: 0;
    transition: opacity 0.3s ease;
}

.avatar-container:hover .avatar-overlay {
    opacity: 1;
}

.btn-delete {
    position: absolute;
    bottom: 0;
    right: -5px;
    width: 32px;
    height: 32px;
    border-radius: 50%;
    padding: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 10;
}

.texto-ayuda {
    color: #6c757d;
    transition: color 0.3s ease;
}

:global(.dark-theme) .texto-ayuda {
    color: #aeb5ce !important;
}
</style>