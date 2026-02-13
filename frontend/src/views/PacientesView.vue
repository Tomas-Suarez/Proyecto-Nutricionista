<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { PacienteService } from '../services/PacienteService';
import type { PacienteResponseDTO } from '../types/dto/response/PacienteResponseDTO';
import type { DataTablePageEvent } from 'primevue/datatable';

import Dialog from 'primevue/dialog';
import RegistroPacienteForm from '../components/nutricionista/RegistroPacienteForm.vue';
import TablaPacientes from '../components/nutricionista/TablaPacientes.vue';

const router = useRouter();
const toast = useToast();

const loading = ref(true);
const pacientes = ref<PacienteResponseDTO[]>([]);
const totalRecords = ref(0);
const busquedaActual = ref('');

const lazyParams = ref({ page: 0, rows: 10 });

const dialogoVisible = ref(false);
const pacienteSeleccionado = ref<PacienteResponseDTO | null>(null);

onMounted(() => {
    cargarPacientes();
});

const cargarPacientes = async () => {
    loading.value = true;
    try {
        const pageNumber = lazyParams.value.page + 1;
        const response = await PacienteService.obtenerPacientesPorNutricionista(
            pageNumber,
            lazyParams.value.rows,
            undefined
        );

        // @ts-ignore
        pacientes.value = response.Items || [];
        // @ts-ignore
        totalRecords.value = response.TotalCount || 0;
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar la lista', life: 3000 });
    } finally {
        loading.value = false;
    }
};

const onPage = (event: DataTablePageEvent) => {
    lazyParams.value = event;
    cargarPacientes();
};

const onBusqueda = (termino: string) => {
    busquedaActual.value = termino;
    lazyParams.value.page = 0;
    cargarPacientes();
};

const onNuevoPaciente = () => {
    pacienteSeleccionado.value = null;
    dialogoVisible.value = true;
};

const onEditarPaciente = (paciente: PacienteResponseDTO) => {
    pacienteSeleccionado.value = { ...paciente };
    dialogoVisible.value = true;
};

const onVerDieta = (id: number) => router.push(`/app/pacientes/${id}/dieta`);
const onVerPesaje = (id: number) => router.push(`/app/pacientes/${id}/pesaje`);

const alGuardarPaciente = async () => {
    dialogoVisible.value = false;
    await cargarPacientes();
};
</script>

<template>
    <div class="card border-0 shadow-sm rounded-4 h-100">
        <div class="card-body p-4">
            
            <TablaPacientes 
                :pacientes="pacientes"
                :totalRecords="totalRecords"
                :loading="loading"
                :rows="lazyParams.rows"
                @page="onPage"
                @busqueda="onBusqueda"
                @nuevo="onNuevoPaciente"
                @editar="onEditarPaciente"
                @ver-dieta="onVerDieta"
                @ver-pesaje="onVerPesaje"
            />

        </div>

        <Dialog 
            v-model:visible="dialogoVisible" 
            modal 
            :header="pacienteSeleccionado ? 'Editar Paciente' : 'Nuevo Paciente'" 
            :style="{ width: '500px' }"
            :draggable="false"
        >
            <RegistroPacienteForm 
                :pacienteEditar="pacienteSeleccionado"
                @guardar="alGuardarPaciente" 
                @cancelar="dialogoVisible = false" 
            />
        </Dialog>
    </div>
</template>

<style scoped>
:global(.dark-theme) .card {
    background-color: #1d1b31 !important;
    color: #fff !important;
}
</style>