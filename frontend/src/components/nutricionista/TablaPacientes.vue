<script setup lang="ts">
import { ref } from 'vue';
import type { PacienteResponseDTO } from '../../types/dto/response/PacienteResponseDTO';
import { EEstadoPaciente } from '../../types/enum/EEstadoPaciente';
import { useToast } from 'primevue/usetoast';

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

const toast = useToast();
const filtros = ref({});
const terminoBusqueda = ref('');
const timeoutBusqueda = ref<any>(null);

const onInputBusqueda = () => {
    if (timeoutBusqueda.value) clearTimeout(timeoutBusqueda.value);
    timeoutBusqueda.value = setTimeout(() => {
        emit('busqueda', terminoBusqueda.value);
    }, 500);
};

const copiarAcceso = (paciente: PacienteResponseDTO) => {
    if (!paciente.TokenAcceso) {
        toast.add({ severity: 'warn', summary: 'Atención', detail: 'El paciente no tiene un token de acceso.', life: 3000 });
        return;
    }
    const urlAcceso = `${window.location.origin}/acceso/${paciente.TokenAcceso}`;
    navigator.clipboard.writeText(urlAcceso).then(() => {
        toast.add({ severity: 'success', summary: 'Copiado', detail: `Link copiado.`, life: 3000 });
    });
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
            <h5 class="fw-bold text-primary m-0"><i class="pi pi-users me-2"></i>Mis Pacientes</h5>
            <p class="text-muted small m-0">Gestión de seguimientos</p>
        </div>
        <div class="d-flex gap-2">
            <span class="p-input-icon-left">
                <i class="pi pi-search" />
                <InputText v-model="terminoBusqueda" placeholder="Buscar..." @input="onInputBusqueda" class="p-inputtext-sm" style="width: 250px;" />
            </span>
            <Button label="Nuevo Paciente" icon="pi pi-plus" class="p-button-primary rounded-3" @click="emit('nuevo')" />
        </div>
    </div>

    <DataTable 
        :value="props.pacientes" :loading="props.loading" lazy paginator :rows="props.rows"
        :totalRecords="props.totalRecords" @page="(e) => emit('page', e)" :rowsPerPageOptions="[5, 10, 20]"
        dataKey="Id_Paciente" class="p-datatable-sm custom-table shadow-sm rounded-3" stripedRows
    >
        <template #empty> No hay pacientes. </template>

        <Column header="Paciente" field="Nombre" style="min-width: 220px">
            <template #body="{ data }">
                <div class="d-flex align-items-center gap-3 py-1">
                    <div class="avatar-circle"><span>{{ data.Nombre?.charAt(0) }}{{ data.Apellido?.charAt(0) }}</span></div>
                    <div class="d-flex flex-column">
                        <span class="fw-bold text-dark-emphasis">{{ data.Nombre }} {{ data.Apellido }}</span>
                        <span class="text-muted small">{{ data.Email }}</span>
                    </div>
                </div>
            </template>
        </Column>

        <Column header="Físico">
            <template #body="{ data }">
                <div class="small d-flex flex-column gap-1">
                    <div><i class="pi pi-arrows-v me-1 text-primary"></i> {{ data.Altura_Cm }} cm</div>
                    <div><i class="pi pi-chart-bar me-1 text-success"></i> {{ data.Peso_Inicial }} kg</div>
                </div>
            </template>
        </Column>

        <Column header="Objetivo">
            <template #body="{ data }">
                <Tag :value="data.Objetivo ? data.Objetivo.Nombre : 'Sin Objetivo'" 
                     :severity="data.Objetivo ? 'info' : 'secondary'" 
                     class="text-xs" rounded />
            </template>
        </Column>

        <Column header="Patologías" style="min-width: 200px">
            <template #body="{ data }">
                <div class="d-flex flex-wrap gap-1 align-items-center">
                    <template v-if="data.Patologias && data.Patologias.length > 0">
                        <Tag v-for="pat in data.Patologias" :key="pat.Id_Patologia" 
                             :value="pat.Nombre" severity="warning" class="text-xs" rounded />
                    </template>
                    <template v-else>
                        <Tag value="Ninguna" severity="secondary" class="text-xs" rounded />
                    </template>
                </div>
            </template>
        </Column>

        <Column header="Acceso" style="min-width: 150px;">
            <template #body="{ data }">
                <div class="d-flex align-items-center gap-2">
                    <Button icon="pi pi-link" class="p-button-rounded p-button-text p-button-primary" @click="copiarAcceso(data)" v-tooltip.top="'Copiar Link'" />
                    <div class="d-flex flex-column">
                        <span class="tiny-text text-muted">PIN</span>
                        <Tag :value="data.CodigoAcceso" severity="secondary" class="font-monospace px-2" />
                    </div>
                </div>
            </template>
        </Column>

        <Column header="Estado">
            <template #body="{ data }">
                <Tag :value="data.Estado" :severity="getSeverity(data.Estado)" class="text-xs" />
            </template>
        </Column>

        <Column header="Acciones">
            <template #body="{ data }">
                <div class="d-flex gap-2">
                    <Button icon="pi pi-chart-line" class="p-button-rounded p-button-outlined p-button-info btn-sm-custom" @click="emit('ver-pesaje', data.Id_Paciente)" v-tooltip.top="'Pesajes'"/>
                    <Button icon="pi pi-apple" class="p-button-rounded p-button-outlined p-button-success btn-sm-custom" @click="emit('ver-dieta', data.Id_Paciente)" v-tooltip.top="'Dietas'"/>
                    <Button icon="pi pi-pencil" class="p-button-rounded p-button-text p-button-secondary btn-sm-custom" @click="emit('editar', data)" v-tooltip.top="'Editar'"/>
                </div>
            </template>
        </Column>
    </DataTable>
</template>

<style scoped>
.avatar-circle {
    width: 38px; height: 38px; border-radius: 50%;
    background-color: #6366f1; color: white;
    display: flex; justify-content: center; align-items: center;
    font-weight: bold; font-size: 0.9rem; text-transform: uppercase;
}
.btn-sm-custom { width: 2.2rem !important; height: 2.2rem !important; }
.tiny-text { font-size: 0.65rem; font-weight: bold; text-transform: uppercase; margin-bottom: -2px; }
.font-monospace { font-family: 'Courier New', monospace; font-weight: bold; letter-spacing: 1px; }
:deep(.p-datatable-sm .p-datatable-tbody > tr > td) { padding: 0.5rem; }
.custom-table { border: 1px solid #e2e8f0; }
</style>