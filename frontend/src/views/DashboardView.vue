<template>
  <div class="min-h-screen bg-gray-50">
    <Toolbar class="mb-4 p-4 shadow-md bg-white border-none rounded-none">
      <template #start>
        <div class="flex items-center gap-2">
          <i class="pi pi-apple text-green-500 text-xl"></i>
          <span class="font-bold text-xl text-gray-800">NutriApp</span>
        </div>
      </template>

      <template #end>
        <div class="flex items-center gap-4">
          <span class="text-sm text-gray-600 hidden md:block">
            Hola, <strong>{{ authStore.usuario?.Email }}</strong>
          </span>
          <Button 
            label="Salir" 
            icon="pi pi-power-off" 
            severity="danger" 
            text 
            @click="handleLogout" 
          />
        </div>
      </template>
    </Toolbar>

    <div class="p-4 max-w-7xl mx-auto">
      
      <Card class="mb-6 shadow-sm">
        <template #title>Panel de Control</template>
        <template #subtitle>
          Estás conectado como <Tag :value="authStore.rolUsuario" severity="info" />
        </template>
      </Card>

      <div v-if="esNutricionista" class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card>
          <template #title>Mis Pacientes</template>
          <template #content>
            <p class="m-0 mb-4 text-gray-600">Administra las dietas y el progreso de tus pacientes.</p>
            <Button label="Ver Pacientes" icon="pi pi-users" />
          </template>
        </Card>

        <Card>
          <template #title>Gestión de Alimentos</template>
          <template #content>
            <p class="m-0 mb-4 text-gray-600">Agrega o edita comidas en la base de datos.</p>
            <Button label="Administrar Comidas" icon="pi pi-list" severity="secondary" />
          </template>
        </Card>
      </div>

      <div v-else-if="esPaciente" class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card>
          <template #title>Mi Dieta del Día</template>
          <template #content>
            <p class="m-0 mb-4 text-gray-600">Revisa qué te toca comer hoy según tu plan.</p>
            <Button label="Ver Dieta" icon="pi pi-calendar" severity="success" />
          </template>
        </Card>

        <Card>
          <template #title>Mi Progreso</template>
          <template #content>
            <p class="m-0 mb-4 text-gray-600">Registra tu peso y ve tu evolución.</p>
            <Button label="Registrar Pesaje" icon="pi pi-chart-line" severity="warning" />
          </template>
        </Card>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStores';
import { ERol } from '../types/enum/ERol';

import Toolbar from 'primevue/toolbar';
import Button from 'primevue/button';
import Card from 'primevue/card';
import Tag from 'primevue/tag';

const authStore = useAuthStore();
const router = useRouter();

const esNutricionista = computed(() => authStore.rolUsuario === ERol.Nutricionista);
const esPaciente = computed(() => authStore.rolUsuario === ERol.Paciente);

const handleLogout = async () => {
  await authStore.logout();
  router.push('/login');
};
</script>