<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { PacienteService } from '../services/PacienteService';
import type { PacienteResponseDTO } from '../types/dto/response/PacienteResponseDTO';
import type { DataTablePageEvent } from 'primevue/datatable';

import TablaPacientes from '../components/nutricionista/TablaPacientes.vue';
import RegistroPacienteForm from '../components/nutricionista/RegistroPacienteForm.vue';
import HistorialPeso from '../components/pacientes/HistorialPeso.vue';
import Dialog from 'primevue/dialog';

const pacientes = ref<PacienteResponseDTO[]>([]);
const totalRecords = ref(0);
const loading = ref(false);
const rows = ref(10);
const first = ref(0);
const busquedaActual = ref('');

const mostrarModal = ref(false);
const mostrarPesajes = ref(false);
const pacienteSeleccionado = ref<PacienteResponseDTO | null>(null);

const cargarPacientes = async (page: number = 1, size: number = 10, search: string = '') => {
    loading.value = true;
    try {
        const res = await PacienteService.obtenerPacientesPorNutricionista(page, size, undefined, search);

        if (res) {
            pacientes.value = (res as any).Items || [];
            totalRecords.value = (res as any).TotalCount || 0;
        }
    } catch (error) {
        console.error(error);
        pacientes.value = [];
    } finally {
        loading.value = false;
    }
};

onMounted(() => cargarPacientes());

const onPage = (event: DataTablePageEvent) => {
    first.value = event.first;
    rows.value = event.rows;
    const page = (event.page ?? 0) + 1;
    cargarPacientes(page, rows.value, busquedaActual.value);
};

const onBusqueda = (termino: string) => {
    busquedaActual.value = termino;
    first.value = 0;
    cargarPacientes(1, rows.value, termino);
};

const abrirNuevo = () => {
    pacienteSeleccionado.value = null;
    mostrarModal.value = true;
};

const abrirEditar = (paciente: PacienteResponseDTO) => {
    pacienteSeleccionado.value = { ...paciente };
    mostrarModal.value = true;
};

const abrirPesajes = (paciente: PacienteResponseDTO) => {
    pacienteSeleccionado.value = paciente;
    mostrarPesajes.value = true;
};

const alGuardar = () => {
    mostrarModal.value = false;
    cargarPacientes(1, rows.value, busquedaActual.value);
};
</script>

<template>
    <div class="p-4">
        <TablaPacientes :pacientes="pacientes" :totalRecords="totalRecords" :loading="loading" :rows="rows"
            @page="onPage" @busqueda="onBusqueda" @nuevo="abrirNuevo" @editar="abrirEditar"
            @ver-pesaje="abrirPesajes" />
        <Dialog v-model:visible="mostrarModal"
            :header="pacienteSeleccionado ? 'Editar Paciente' : 'Registrar Nuevo Paciente'" :style="{ width: '50vw' }"
            modal>
            <RegistroPacienteForm :pacienteEdicion="pacienteSeleccionado" @guardar="alGuardar"
                @cancelar="mostrarModal = false" />
        </Dialog>

        <Dialog v-model:visible="mostrarPesajes" :header="'Control de Peso'" :style="{ width: '90vw' }"
            :maximizable="true" modal :dismissableMask="true">
            <HistorialPeso v-if="mostrarPesajes && pacienteSeleccionado" :paciente="pacienteSeleccionado" />
        </Dialog>
    </div>
</template>