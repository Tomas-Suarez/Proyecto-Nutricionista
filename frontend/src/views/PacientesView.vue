<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { PacienteService } from '../services/PacienteService';
import type { PacienteResponseDTO } from '../types/dto/response/PacienteResponseDTO';
import { EEstadoPaciente } from '../types/enum/EEstadoPaciente';
import type { DataTablePageEvent } from 'primevue/datatable';
import { useToast } from 'primevue/usetoast';

import TablaPacientes from '../components/nutricionista/TablaPacientes.vue';
import RegistroPacienteForm from '../components/nutricionista/RegistroPacienteForm.vue';
import HistorialPeso from '../components/pacientes/HistorialPeso.vue';
import GestionDieta from '../components/dieta/GestionDieta.vue'
import Dialog from 'primevue/dialog';

const pacientes = ref<PacienteResponseDTO[]>([]);
const totalRecords = ref(0);
const loading = ref(false);
const rows = ref(10);
const first = ref(0);
const busquedaActual = ref('');

const mostrarModal = ref(false);
const mostrarPesajes = ref(false);
const mostrarDieta = ref(false);
const pacienteSeleccionado = ref<PacienteResponseDTO | null>(null);

const toast = useToast();

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

const abrirDieta = (id: number) => {
    const paciente = pacientes.value.find(p => p.Id_Paciente === id);
    if (paciente) {
        pacienteSeleccionado.value = paciente;
        mostrarDieta.value = true;
    }
};

const handleCambiarEstado = async (paciente: PacienteResponseDTO) => {
    try {
        const nuevoEstado = (paciente.Estado === 'Activo' || paciente.Estado === EEstadoPaciente.Activo.toString()) 
            ? EEstadoPaciente.Inactivo 
            : EEstadoPaciente.Activo;
            
        await PacienteService.cambiarEstado(paciente.Id_Paciente, nuevoEstado);
        toast.add({ severity: 'success', summary: 'Éxito', detail: 'Estado actualizado correctamente', life: 3000 });
        
        const currentPage = Math.floor(first.value / rows.value) + 1;
        cargarPacientes(currentPage, rows.value, busquedaActual.value);
    } catch (error) {
        console.error(error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cambiar el estado', life: 3000 });
    }
};

const alGuardar = () => {
    mostrarModal.value = false;
    const currentPage = Math.floor(first.value / rows.value) + 1;
    cargarPacientes(currentPage, rows.value, busquedaActual.value);
};
</script>

<template>
    <div class="card shadow-sm border-0 rounded-4 p-4">
        <TablaPacientes 
            :pacientes="pacientes" 
            :totalRecords="totalRecords" 
            :loading="loading" 
            :rows="rows"
            @page="onPage" 
            @busqueda="onBusqueda" 
            @nuevo="abrirNuevo" 
            @editar="abrirEditar"
            @ver-pesaje="abrirPesajes" 
            @ver-dieta="abrirDieta" 
            @cambiar-estado="handleCambiarEstado" 
        />

        <Dialog v-model:visible="mostrarModal"
            :header="pacienteSeleccionado ? 'Editar Paciente' : 'Registrar Nuevo Paciente'" 
            :style="{ width: '50vw' }"
            modal>
            <RegistroPacienteForm 
                :pacienteEdicion="pacienteSeleccionado" 
                @guardar="alGuardar"
                @cancelar="mostrarModal = false" 
            />
        </Dialog>

        <Dialog v-model:visible="mostrarPesajes" 
            :header="'Control de Peso'" 
            :style="{ width: '90vw' }"
            :maximizable="true" 
            modal 
            :dismissableMask="true">
            <HistorialPeso v-if="mostrarPesajes && pacienteSeleccionado" :paciente="pacienteSeleccionado" />
        </Dialog>

        <Dialog v-model:visible="mostrarDieta" 
            :header="'Menu de dieta'" 
            :style="{ width: '95vw', height: '90vh' }"
            :maximizable="true" 
            modal 
            :dismissableMask="true">
            <GestionDieta v-if="mostrarDieta && pacienteSeleccionado" :paciente="pacienteSeleccionado" />
        </Dialog>
    </div>
</template>