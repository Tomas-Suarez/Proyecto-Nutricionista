<template>
  <div class="flex justify-center items-center min-h-screen bg-surface-100 dark:bg-surface-900 p-4">
    <LoginForm 
      :loading="loading" 
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

const handleLogin = async (credenciales: UsuarioRequestDTO) => {
  loading.value = true;
  try {
    await authStore.login(credenciales);
    router.push('/dashboard');
  } catch (error) {
    console.error("Error de login:", error);
  } finally {
    loading.value = false;
  }
};
</script>