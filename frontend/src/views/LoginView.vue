<template>
  <div class="d-flex justify-content-center align-items-center min-vh-100 w-100 login-bg">
    
    <LoginForm 
      :loading="loading" 
      :error-message="errorMessage"
      @submit="handleLogin" 
    />

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStores';
import type { UsuarioRequestDTO } from '../types/dto/request/UsuarioRequestDTO';
import LoginForm from '../components/LoginForm.vue';

const authStore = useAuthStore();
const router = useRouter();
const loading = ref(false);
const errorMessage = ref('');

const handleLogin = async (credenciales: UsuarioRequestDTO) => {
  loading.value = true;
  errorMessage.value = '';

  try {
    await authStore.login(credenciales);
    router.push('/dashboard');
  } catch (error: any) {
    console.error("Error de login:", error);
    
    if (error.response?.data?.detail) {
      errorMessage.value = error.response.data.detail;
    } else if (error.response?.data?.message) {
      errorMessage.value = error.response.data.message;
    } else {
      errorMessage.value = 'Credenciales incorrectas o error en el servidor.';
    }
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