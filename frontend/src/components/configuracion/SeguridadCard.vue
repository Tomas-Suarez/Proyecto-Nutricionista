<script setup lang="ts">
import { ref, computed } from 'vue';
import { UsuarioService } from '../../services/UsuarioService';

const cargando = ref(false);
const mensajeExito = ref('');
const error = ref('');

const form = ref({
    PasswordActual: '',
    NuevaPassword: '',
    ConfirmarPassword: ''
});

const passwordsCoinciden = computed(() => {
    return form.value.NuevaPassword === form.value.ConfirmarPassword;
});

const cambiarPassword = async () => {
    if (!passwordsCoinciden.value) {
        error.value = "Las contraseñas nuevas no coinciden.";
        return;
    }
    if (form.value.NuevaPassword.length < 6) {
        error.value = "La contraseña debe tener al menos 6 caracteres.";
        return;
    }

    cargando.value = true;
    error.value = '';
    mensajeExito.value = '';

    try {
        await UsuarioService.cambiarMiPassword({
            PasswordActual: form.value.PasswordActual,
            PasswordNueva: form.value.NuevaPassword,
        });

        mensajeExito.value = "¡Contraseña actualizada correctamente!";
        form.value = { PasswordActual: '', NuevaPassword: '', ConfirmarPassword: '' };
        
        setTimeout(() => mensajeExito.value = '', 3000);

    } catch (err: any) {
        console.error(err);
        error.value = err.response?.data?.message || "Error al actualizar la contraseña.";
    } finally {
        cargando.value = false;
    }
};
</script>

<template>
    <div class="card border-0 shadow-sm rounded-4 h-100">
        <div class="card-body p-4">
            <h5 class="card-title fw-bold mb-3 text-danger">
                <i class="pi pi-lock me-2"></i>Seguridad
            </h5>
            <p class="col-md-6 mb-4">Asegúrate de usar una contraseña segura que no uses en otros sitios.</p>

            <form @submit.prevent="cambiarPassword">
                
                <div v-if="error" class="alert alert-danger py-2 text-small animate-fade">
                    <i class="pi pi-exclamation-circle me-1"></i> {{ error }}
                </div>
                
                <div v-if="mensajeExito" class="alert alert-success py-2 text-small animate-fade">
                    <i class="pi pi-check-circle me-1"></i> {{ mensajeExito }}
                </div>

                <div class="row align-items-end">
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Contraseña Actual</label>
                        <input type="password" class="form-control" v-model="form.PasswordActual" required>
                    </div>

                    <div class="col-md-4 mb-3">
                        <label class="form-label">Nueva Contraseña</label>
                        <input type="password" 
                               class="form-control" 
                               :class="{'is-invalid': form.NuevaPassword && !passwordsCoinciden}"
                               v-model="form.NuevaPassword" required>
                    </div>

                    <div class="col-md-4 mb-3">
                        <label class="form-label">Confirmar Nueva</label>
                        <input type="password" 
                               class="form-control" 
                               :class="{'is-invalid': form.ConfirmarPassword && !passwordsCoinciden}"
                               v-model="form.ConfirmarPassword" required>
                        <div class="invalid-feedback">No coinciden</div>
                    </div>
                </div>

                <div class="d-flex justify-content-end mt-2">
                    <button type="submit" 
                            class="btn btn-outline-danger d-flex align-items-center gap-2"
                            :disabled="cargando || !form.PasswordActual || !form.NuevaPassword">
                        <i v-if="cargando" class="pi pi-spin pi-spinner"></i>
                        {{ cargando ? 'Actualizando...' : 'Cambiar Contraseña' }}
                    </button>
                </div>
            </form>
        </div>
    </div>
</template>

<style scoped>
.animate-fade {
    animation: fadeIn 0.3s ease-in-out;
}
@keyframes fadeIn {
    from { opacity: 0; transform: translateY(-5px); }
    to { opacity: 1; transform: translateY(0); }
}
.text-small {
    font-size: 0.9rem;
}
</style>