<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { NutricionistaService } from '../../services/NutricionistaService';
import { useConfirm } from "primevue/useconfirm";
import { useAuthStore } from '../../stores/authStores';

const confirm = useConfirm();
const authStore = useAuthStore();
const cargando = ref(false);
const emailMostrar = ref('');

const form = ref({
    Nombre: '',
    Apellido: '',
    Matricula: '',
    Telefono: ''
});

onMounted(async () => {
    cargando.value = true;
    try {
        const perfil = await NutricionistaService.obtenerMiPerfil();
        
        emailMostrar.value = perfil.Email;

        Object.assign(form.value, perfil);
        
    } catch (error) {
        console.error("Error al cargar perfil:", error);
    } finally {
        cargando.value = false;
    }
});

const guardar = async () => {
    confirm.require({
        message: '¿Estás seguro de que deseas guardar los cambios?',
        header: 'Confirmación',
        icon: 'pi pi-exclamation-triangle',
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
        
        confirm.require({
            header: 'Éxito',
            message: 'Datos actualizados correctamente',
            icon: 'pi pi-check-circle',
            acceptLabel: 'Entendido',
            rejectClass: 'd-none',
        });

    } catch (error) {
        confirm.require({
            header: 'Error',
            message: 'No se pudieron guardar los cambios',
            icon: 'pi pi-times-circle',
            acceptLabel: 'Cerrar',
            rejectClass: 'd-none',
            acceptClass: 'p-button-danger'
        });
        console.error(error);
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
                </h5>
                
                <div class="d-flex align-items-center mb-4">
                    <div class="avatar-circle me-3">
                        {{ form.Nombre ? form.Nombre.charAt(0).toUpperCase() : (emailMostrar.charAt(0).toUpperCase() || 'U') }}
                    </div>
                </div>

                <form @submit.prevent="guardar">
                    <div class="mb-3">
                        <label class="col-md-6 mb-3">Correo Electrónico</label>
                        <input type="email" class="form-control" v-model="emailMostrar" readonly disabled>
                    </div>

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
                            <input type="text" class="form-control" v-model="form.Telefono" placeholder="Ej: +54 9 11...">
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Matrícula</label>
                            <input type="text" class="form-control" v-model="form.Matricula" placeholder="Ej: MN-1234">
                        </div>
                    </div>
                    
                    <button type="submit" class="btn btn-primary w-100 rounded-3 mt-2" :disabled="cargando">
                        {{ cargando ? 'Guardando...' : 'Guardar Cambios' }}
                    </button>
                </form>
            </div>
        </div>
    </div>
</template>

<style scoped>
.avatar-circle {
    width: 90px;
    height: 90px;
    background-color: var(--primary-color, #695CFE);
    color: white;
    font-size: 2.5rem; 
    font-weight: bold;
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 50%;
}
</style>