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
    (e: 'ver-pesaje', paciente: PacienteResponseDTO): void; 
    (e: 'cambiar-estado', paciente: PacienteResponseDTO): void;
}>();

const toast = useToast();
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

const esActivo = (estado: string) => {
    return estado === 'Activo' || estado === EEstadoPaciente.Activo.toString();
};

const getSeverity = (estado: string) => {
    if (esActivo(estado)) return 'success';
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
                <i class="pi pi-search text-muted" />
                <InputText v-model="terminoBusqueda" placeholder="Buscar..." @input="onInputBusqueda" class="p-inputtext-sm input-dark" style="width: 250px;" />
            </span>
            <Button label="Nuevo Paciente" icon="pi pi-plus" class="btn-primary-custom rounded-3" @click="emit('nuevo')" />
        </div>
    </div>

    <DataTable 
        :value="props.pacientes" :loading="props.loading" lazy paginator :rows="props.rows"
        :totalRecords="props.totalRecords" @page="(e) => emit('page', e)" :rowsPerPageOptions="[5, 10, 20]"
        dataKey="Id_Paciente" class="p-datatable-sm dark-table"
    >
        <template #empty> <span class="text-muted">No hay pacientes.</span> </template>

        <Column header="Paciente" field="Nombre" style="min-width: 220px">
            <template #body="{ data }">
                <div class="d-flex align-items-center gap-3 py-1">
                    <div class="avatar-circle"><span>{{ data.Nombre?.charAt(0) }}{{ data.Apellido?.charAt(0) }}</span></div>
                    <div class="d-flex flex-column">
                        <span class="fw-bold text-main">{{ data.Nombre }} {{ data.Apellido }}</span>
                        <span class="text-muted small">{{ data.Email }}</span>
                    </div>
                </div>
            </template>
        </Column>

        <Column header="Físico">
            <template #body="{ data }">
                <div class="small d-flex flex-column gap-1 text-muted">
                    <div><i class="pi pi-arrows-v me-1 text-primary"></i> {{ data.Altura_Cm }} cm</div>
                    <div><i class="pi pi-chart-bar me-1 text-success"></i> {{ data.PesoActual }} kg</div>
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
                        <Tag :value="data.CodigoAcceso" severity="secondary" class="font-monospace px-2 text-main" style="background-color: var(--body-bg) !important;" />
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
                    <Button icon="pi pi-chart-line" class="p-button-rounded p-button-outlined p-button-info btn-sm-custom" @click="emit('ver-pesaje', data)" v-tooltip.top="'Pesajes'"/>
                    
                    <Button icon="pi pi-apple" class="p-button-rounded p-button-outlined p-button-success btn-sm-custom" @click="emit('ver-dieta', data.Id_Paciente)" v-tooltip.top="'Dietas'"/>
                    
                    <Button icon="pi pi-pencil" class="p-button-rounded p-button-text p-button-secondary btn-sm-custom" @click="emit('editar', data)" v-tooltip.top="'Editar'"/>

                    <Button 
                        :icon="esActivo(data.Estado) ? 'pi pi-ban' : 'pi pi-check-circle'" 
                        class="p-button-rounded p-button-text btn-sm-custom"
                        :class="esActivo(data.Estado) ? 'p-button-danger' : 'p-button-success'"
                        @click="emit('cambiar-estado', data)" 
                        v-tooltip.top="esActivo(data.Estado) ? 'Desactivar' : 'Activar'"
                    />
                </div>
            </template>
        </Column>
    </DataTable>
</template>

<style scoped>
/* Colores consistentes con tu AppLayout */
.text-main { color: var(--text-primary, #ffffff) !important; }
.text-muted { color: var(--text-secondary, #aeb5ce) !important; }

/* Input de búsqueda oscuro */
.input-dark {
    background-color: var(--body-bg, #0c0b16) !important;
    border: 1px solid var(--border-color, #2d2b45) !important;
    color: var(--text-primary, #ffffff) !important;
}
.input-dark:focus {
    border-color: var(--accent-color, #695CFE) !important;
    box-shadow: none !important;
}

/* Botón principal (morado) */
.btn-primary-custom {
    background-color: var(--accent-color, #695CFE) !important;
    border: none !important;
    color: white !important;
}

.avatar-circle {
    width: 38px; height: 38px; border-radius: 50%;
    background-color: var(--accent-color, #695CFE); color: white;
    display: flex; justify-content: center; align-items: center;
    font-weight: bold; font-size: 0.9rem; text-transform: uppercase;
}
.btn-sm-custom { width: 2.2rem !important; height: 2.2rem !important; }
.tiny-text { font-size: 0.65rem; font-weight: bold; text-transform: uppercase; margin-bottom: -2px; }
.font-monospace { font-family: 'Courier New', monospace; font-weight: bold; letter-spacing: 1px; }

/* OVERRIDES PARA QUITAR LOS BORDES Y FONDOS BLANCOS DE PRIMEVUE */
:deep(.dark-table) {
    background: transparent !important;
    border: none !important;
}
:deep(.dark-table .p-datatable-header) {
    background: transparent !important;
    border: none !important;
}
:deep(.dark-table .p-datatable-thead > tr > th) {
    background-color: transparent !important;
    border-top: none !important;
    border-left: none !important;
    border-right: none !important;
    border-bottom: 1px solid var(--border-color, #2d2b45) !important;
    color: var(--text-secondary, #aeb5ce) !important;
    font-weight: 600;
}
:deep(.dark-table .p-datatable-tbody > tr) {
    background-color: transparent !important;
    color: var(--text-primary, #ffffff) !important;
}
/* Efecto Striped pero con tus variables oscuras */
:deep(.dark-table.p-datatable-striped .p-datatable-tbody > tr:nth-child(even)) {
    background-color: rgba(255, 255, 255, 0.02) !important; /* Un gris muuuy sutil */
}
:deep(.dark-table .p-datatable-tbody > tr > td) {
    border-top: none !important;
    border-left: none !important;
    border-right: none !important;
    border-bottom: 1px solid var(--border-color, #2d2b45) !important;
    padding: 0.75rem 0.5rem;
}
:deep(.dark-table .p-paginator) {
    background-color: transparent !important;
    border: none !important;
    border-top: 1px solid var(--border-color, #2d2b45) !important;
    padding-top: 1rem;
}
:deep(.dark-table .p-paginator .p-paginator-pages .p-paginator-page),
:deep(.dark-table .p-paginator .p-paginator-first),
:deep(.dark-table .p-paginator .p-paginator-prev),
:deep(.dark-table .p-paginator .p-paginator-next),
:deep(.dark-table .p-paginator .p-paginator-last) {
    color: var(--text-secondary, #aeb5ce) !important;
    background: transparent !important;
    border: none !important;
}
:deep(.dark-table .p-paginator .p-paginator-pages .p-paginator-page.p-highlight) {
    background-color: var(--accent-color, #695CFE) !important;
    color: white !important;
    border-radius: 5px;
}
</style>