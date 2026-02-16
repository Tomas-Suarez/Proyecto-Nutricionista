<script setup lang="ts">
import Card from 'primevue/card';
import Button from 'primevue/button';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import Divider from 'primevue/divider';

const props = defineProps<{
    historial: any[],
    loading: boolean,
    totalRegistros: number,
    rows: number
}>();

const emit = defineEmits(['crear', 'editar', 'eliminar', 'pageChange']);

const onPage = (event: any) => {
    emit('pageChange', event);
};
</script>

<template>
    <Card class="shadow-sm border-0 bg-surface-50">
        <template #content>
            <div class="d-flex justify-content-between align-items-center">
                <h5 class="fw-bold text-primary m-0"><i class="pi pi-history me-2"></i>Historial de Planes</h5>
                <Button label="Crear Nuevo" icon="pi pi-plus" severity="success" @click="emit('crear')" />
            </div>
            <Divider />
            
            <DataTable 
                :value="historial" 
                :loading="loading" 
                lazy 
                paginator
                :rows="rows" 
                :totalRecords="totalRegistros" 
                @page="onPage" 
                class="tiny-table"
                sortField="activa" 
                :sortOrder="-1"
            >
                <template #empty><div class="text-center text-muted">Sin planes registrados.</div></template>
                
                <Column field="nombre" header="PLAN">
                    <template #body="{ data }"><span class="fw-bold">{{ data.nombre || data.Nombre }}</span></template>
                </Column>
                <Column header="DESCRIPCIÓN">
                    <template #body="{ data }">
                        <i v-if="data.descripcion || data.Descripcion" class="pi pi-comment text-primary" v-tooltip.top="data.descripcion || data.Descripcion"></i>
                        <span v-else class="text-muted small">-</span>
                    </template>
                </Column>
                <Column field="fecha_Inicio" header="DESDE">
                    <template #body="{ data }">{{ new Date(data.fecha_Inicio || data.Fecha_Inicio).toLocaleDateString('es-AR') }}</template>
                </Column>
                <Column field="fecha_Fin" header="HASTA">
                    <template #body="{ data }">{{ new Date(data.fecha_Fin || data.Fecha_Fin).toLocaleDateString('es-AR') }}</template>
                </Column>
                
                <Column field="activa" header="ESTADO" sortable>
                    <template #body="{ data }">
                        <Tag :value="(data.activa || data.Activa) ? 'Activo' : 'Inactivo'" :severity="(data.activa || data.Activa) ? 'success' : 'secondary'" />
                    </template>
                </Column>
                
                <Column header="ACCIONES">
                    <template #body="{ data }">
                        <div class="d-flex gap-2">
                            <Button icon="pi pi-pencil" text rounded severity="info" @click="emit('editar', data)" />
                            <Button icon="pi pi-trash" text rounded severity="danger" @click="emit('eliminar', data)" />
                        </div>
                    </template>
                </Column>
            </DataTable>
        </template>
    </Card>
</template>

<style scoped>
:deep(.tiny-table .p-datatable-thead > tr > th) { background: transparent; padding: 0.5rem; font-size: 0.75rem; text-transform: uppercase; }
:deep(.tiny-table .p-datatable-tbody > tr > td) { padding: 0.5rem; font-size: 0.85rem; }
</style>