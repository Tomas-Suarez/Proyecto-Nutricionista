<script setup lang="ts">
import { ref } from 'vue';
import type { PacienteResponseDTO } from '../../types/dto/response/PacienteResponseDTO';
import { EEstadoPaciente } from '../../types/enum/EEstadoPaciente';

import DataTable, { type DataTablePageEvent } from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Tag from 'primevue/tag';
import InputText from 'primevue/inputtext';

const props = defineProps<{
    pacientes: PacienteResponseDTO[];
    totalRecords: number;
    loading: boolean;
    rows: number;
}>();

const emit = defineEmits<{
    (e: 'page', event: DataTablePageEvent): void;
    (e: 'busqueda', termino: string): void;
    (e: 'nuevo'): void;
    (e: 'editar', paciente: PacienteResponseDTO): void;
    (e: 'ver-dieta', id: number): void;
    (e: 'ver-pesaje', id: number): void;
}>();

const filtros = ref({});
const terminoBusqueda = ref('');
const timeoutBusqueda = ref<any>(null);

const onInputBusqueda = () => {
    if (timeoutBusqueda.value) clearTimeout(timeoutBusqueda.value);
    timeoutBusqueda.value = setTimeout(() => {
        emit('busqueda', terminoBusqueda.value);
    }, 500);
};

const getSeverity = (estado: string) => {
    if (estado === 'Activo' || estado === EEstadoPaciente.Activo.toString()) return 'success';
    if (estado === 'Inactivo' || estado === EEstadoPaciente.Inactivo.toString()) return 'danger';
    return 'info';
};
</script>

<template>
    <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
        <div>
            <h5 class="fw-bold text-primary m-0">
                <i class="pi pi-users me-2"></i>Mis Pacientes
            </h5>
            <p class="text-muted small m-0">Gestiona dietas y seguimientos</p>
        </div>

        <div class="d-flex gap-2">
            <span class="p-input-icon-left">
                <i class="pi pi-search" />
                <InputText 
                    v-model="terminoBusqueda" 
                    placeholder="Buscar..." 
                    @input="onInputBusqueda"
                    class="p-inputtext-sm" 
                    style="width: 200px;"
                />
            </span>
            <Button 
                label="Nuevo Paciente" 
                icon="pi pi-plus" 
                class="p-button-primary rounded-3"
                @click="emit('nuevo')"
            />
        </div>
    </div>

    <DataTable 
        v-model:filters="filtros"
        :value="props.pacientes" 
        :loading="props.loading"
        lazy
        paginator 
        :rows="props.rows"
        :totalRecords="props.totalRecords"
        @page="(e) => emit('page', e)"
        :rowsPerPageOptions="[5, 10, 20]"
        dataKey="Id_Paciente"
        class="p-datatable-sm custom-table"
        stripedRows
    >
        <template #empty> No se encontraron pacientes. </template>

        <Column header="Paciente" field="Nombre" style="min-width: 250px">
            <template #body="{ data }">
                <div class="d-flex align-items-center gap-3 h-100">
                    <div class="avatar-circle shadow-sm">
                        <span>{{ data.Nombre ? data.Nombre.charAt(0) : '' }}{{ data.Apellido ? data.Apellido.charAt(0) : '' }}</span>
                    </div>
                    <div class="d-flex flex-column justify-content-center">
                        <span class="fw-bold text-dark-emphasis">{{ data.Nombre }} {{ data.Apellido }}</span>
                        <span class="text-muted small">{{ data.Email }}</span>
                    </div>
                </div>
            </template>
        </Column>

        <Column header="Físico">
            <template #body="{ data }">
                <div class="small d-flex flex-column justify-content-center h-100">
                    <div><i class="pi pi-arrows-v me-1 text-primary"></i> {{ data.Altura_Cm }} cm</div>
                    <div class="mt-1"><i class="pi pi-chart-bar me-1 text-success"></i> {{ data.Peso_Inicial }} kg</div>
                </div>
            </template>
        </Column>

        <Column header="Objetivo" field="Objetivo">
            <template #body="{ data }">
                <div class="d-flex align-items-center h-100">
                        <Tag :value="data.Objetivo ? data.Objetivo.Nombre : 'Sin Objetivo'" severity="info" class="text-xs" rounded />
                </div>
            </template>
        </Column>

        <Column header="Patologías" style="max-width: 200px;">
            <template #body="{ data }">
                <div class="d-flex flex-wrap gap-1 align-items-center h-100">
                    <template v-if="data.Patologias && data.Patologias.length > 0">
                        <Tag v-for="pat in data.Patologias" :key="pat.Id_Patologia" :value="pat.Nombre" severity="warning" class="text-xs" rounded />
                    </template>
                    <template v-else>
                        <Tag value="Sin Patologías" severity="info" class="text-xs" rounded />
                    </template>
                </div>
            </template>
        </Column>

        <Column header="Estado" field="Estado">
            <template #body="{ data }">
                <div class="d-flex justify-content-center align-items-center h-100">
                    <Tag :value="data.Estado" :severity="getSeverity(data.Estado)" class="text-xs" style="transform: scale(0.9)" />
                </div>
            </template>
        </Column>

        <Column header="Acciones" style="min-width: 180px;">
            <template #body="{ data }">
                <div class="d-flex gap-2 justify-content-end align-items-center h-100">
                    <Button icon="pi pi-chart-line" v-tooltip.top="'Ver Pesajes'" class="p-button-rounded p-button-outlined p-button-info btn-sm-custom" @click="emit('ver-pesaje', data.Id_Paciente)" />
                    <Button icon="pi pi-apple" v-tooltip.top="'Ver Dieta'" class="p-button-rounded p-button-outlined p-button-success btn-sm-custom" @click="emit('ver-dieta', data.Id_Paciente)" />
                    <Button icon="pi pi-pencil" v-tooltip.top="'Editar'" class="p-button-rounded p-button-text p-button-secondary btn-sm-custom" @click="emit('editar', data)" />
                </div>
            </template>
        </Column>
    </DataTable>
</template>

<style scoped>
:deep(.p-datatable-thead > tr > th),
:deep(.p-datatable-tbody > tr > td) {
    vertical-align: middle !important;
}
.avatar-circle {
    width: 40px; height: 40px; border-radius: 50%;
    background-color: var(--primary-color); color: white;
    display: flex; justify-content: center; align-items: center;
    font-weight: bold; overflow: hidden; font-size: 1.1rem; text-transform: uppercase;
}
.btn-sm-custom { width: 2.5rem !important; height: 2.5rem !important; padding: 0 !important; }

:global(.dark-theme) .text-dark-emphasis { color: #fff !important; }
</style>