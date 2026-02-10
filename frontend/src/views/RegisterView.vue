<template>
  <div class="d-flex justify-content-center align-items-center min-vh-100 w-100 login-bg">
    
    <RegistroNutricionistaForm 
      :loading="loading" 
      @submit="handleRegistro" 
    />
    
    <Toast />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import RegistroNutricionistaForm from '../components/RegistroNutricionistaForm.vue';
import { NutricionistaService } from '../services/NutricionistaService';
import type { RegistroNutricionistaDTO } from '../types/dto/request/RegistroNutricionistaDTO';

import Toast from 'primevue/toast';

const router = useRouter();
const toast = useToast();
const loading = ref(false);

const handleRegistro = async (datos: RegistroNutricionistaDTO) => {
  loading.value = true;
  try {
    await NutricionistaService.registrar(datos);
    
    toast.add({ 
        severity: 'success', 
        summary: '¡Éxito!', 
        detail: 'Cuenta creada correctamente. Inicia sesión.', 
        life: 3000 
    });

    setTimeout(() => {
        router.push('/login');
    }, 1500);

  } catch (error: any) {
    let mensajeMostrar = 'No se pudo crear la cuenta. Intente nuevamente.';

    if (error.response && error.response.data && error.response.data.detail) {
        mensajeMostrar = error.response.data.detail;
    }

    console.error("Error de registro:", error);
    
    toast.add({ 
        severity: 'error', 
        summary: 'Error', 
        detail: mensajeMostrar,
        life: 4000 
    });
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.login-bg {
    background-color: var(--surface-ground, #121212); 
    
}
</style>