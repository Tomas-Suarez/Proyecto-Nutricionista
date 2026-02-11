<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import { ObjetivoService } from '../../services/ObjetivoService';
import type { ObjetivoResponseDTO } from '../../types/dto/response/ObjetivoResponseDTO';

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';

const toast = useToast();
const confirm = useConfirm();

const objetivos = ref<ObjetivoResponseDTO[]>([]);
const cargando = ref(true);
const dialogoVisible = ref(false);
const enviando = ref(false);

const esEdicion = ref(false);
const idEnEdicion = ref<number | null>(null);
const nombreObjetivo = ref('');

onMounted(async () => {
    await cargarObjetivos();
});

const cargarObjetivos = async () => {
    cargando.value = true;
    try {
        objetivos.value = await ObjetivoService.listarTodas();
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los objetivos', life: 3000 });
    } finally {
        cargando.value = false;
    }
};

const abrirCrear = () => {
    esEdicion.value = false;
    idEnEdicion.value = null;
    nombreObjetivo.value = '';
    dialogoVisible.value = true;
};

const abrirEditar = (item: ObjetivoResponseDTO) => {
    esEdicion.value = true;
    idEnEdicion.value = item.Id_Objetivo;
    nombreObjetivo.value = item.Nombre;
    dialogoVisible.value = true;
};

const guardar = async () => {
    if (!nombreObjetivo.value.trim()) {
        toast.add({ severity: 'warn', summary: 'Atención', detail: 'El nombre es obligatorio', life: 3000 });
        return;
    }

    enviando.value = true;
    try {
        if (esEdicion.value && idEnEdicion.value) {
            await ObjetivoService.modificar(idEnEdicion.value, { nombre: nombreObjetivo.value });
            toast.add({ severity: 'success', summary: 'Actualizado', detail: 'Objetivo modificado correctamente', life: 3000 });
        } else {
            await ObjetivoService.crear({ nombre: nombreObjetivo.value });
            toast.add({ severity: 'success', summary: 'Creado', detail: 'Objetivo creado correctamente', life: 3000 });
        }
        
        dialogoVisible.value = false;
        await cargarObjetivos(); 

    } catch (error: any) {
        const mensajeError = error.response?.data?.detail || 'Ocurrió un error al guardar';
        toast.add({ severity: 'error', summary: 'Error', detail: mensajeError, life: 3000 });
    } finally {
        enviando.value = false;
    }
};

const confirmarEliminar = (item: ObjetivoResponseDTO) => {
    confirm.require({
        message: `¿Estás seguro de eliminar el objetivo "${item.Nombre}"?`,
        header: 'Confirmar Eliminación',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                await ObjetivoService.eliminar(item.Id_Objetivo);
                toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Objetivo eliminado', life: 3000 });
                objetivos.value = objetivos.value.filter(o => o.Id_Objetivo !== item.Id_Objetivo);
            } catch (error: any) {
                const mensajeError = error.response?.data?.detail || 'No se pudo eliminar (quizás está en uso)';
                toast.add({ severity: 'error', summary: 'Error', detail: mensajeError, life: 3000 });
            }
        }
    });
};
</script>

<template>
    <div>
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h4 class="fw-bold text-primary m-0">
                <i class="pi pi-bullseye me-2"></i>Gestión de Objetivos
            </h4>
            <Button 
                label="Nuevo Objetivo" 
                icon="pi pi-plus" 
                severity="primary" 
                rounded 
                @click="abrirCrear" 
            />
        </div>

        <DataTable 
            :value="objetivos" 
            :loading="cargando" 
            paginator 
            :rows="10" 
            :rowsPerPageOptions="[5, 10, 20]"
            tableStyle="min-width: 50rem"
            class="p-datatable-sm"
        >
            <template #empty>No se encontraron objetivos.</template>

            <Column field="Id_Objetivo" header="ID" sortable style="width: 10%"></Column>
            
            <Column field="Nombre" header="Nombre" sortable style="width: 70%">
                <template #body="slotProps">
                    <span class="fw-bold">{{ slotProps.data.Nombre }}</span>
                </template>
            </Column>

            <Column header="Acciones" style="width: 20%; text-align: center">
                <template #body="slotProps">
                    <div class="d-flex gap-2 justify-content-center">
                        <Button 
                            icon="pi pi-pencil" 
                            severity="info" 
                            text 
                            rounded 
                            v-tooltip.top="'Editar'"
                            @click="abrirEditar(slotProps.data)" 
                        />
                        <Button 
                            icon="pi pi-trash" 
                            severity="danger" 
                            text 
                            rounded 
                            v-tooltip.top="'Eliminar'"
                            @click="confirmarEliminar(slotProps.data)" 
                        />
                    </div>
                </template>
            </Column>
        </DataTable>

        <Dialog 
            v-model:visible="dialogoVisible" 
            modal 
            :header="esEdicion ? 'Editar Objetivo' : 'Nuevo Objetivo'" 
            :style="{ width: '400px' }"
        >
            <div class="d-flex flex-column gap-3 mt-2">
                <div>
                    <label for="nombre" class="fw-bold mb-1">Nombre</label>
                    <InputText 
                        id="nombre" 
                        v-model="nombreObjetivo" 
                        class="w-100" 
                        placeholder="Ej: Bajar de peso, Aumentar masa..." 
                        autofocus
                    />
                </div>
            </div>

            <template #footer>
                <Button label="Cancelar" icon="pi pi-times" text @click="dialogoVisible = false" />
                <Button 
                    :label="esEdicion ? 'Actualizar' : 'Guardar'" 
                    icon="pi pi-check" 
                    severity="primary" 
                    :loading="enviando" 
                    @click="guardar" 
                />
            </template>
        </Dialog>
    </div>
</template>

<style scoped>
:deep(.p-datatable-header) {
    background: transparent;
    border: none;
}
</style>