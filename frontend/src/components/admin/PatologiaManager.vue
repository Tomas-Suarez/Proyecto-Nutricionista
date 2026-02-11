<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import { PatologiaService } from '../../services/PatologiaService';
import type { PatologiaResponseDTO } from '../../types/dto/response/PatologiaResponseDTO';

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';

const toast = useToast();
const confirm = useConfirm();

const patologias = ref<PatologiaResponseDTO[]>([]);
const cargando = ref(true);
const dialogoVisible = ref(false);
const enviando = ref(false);

const esEdicion = ref(false);
const idEnEdicion = ref<number | null>(null);
const nombrePatologia = ref('');

onMounted(async () => {
    await cargarPatologias();
});

const cargarPatologias = async () => {
    cargando.value = true;
    try {
        patologias.value = await PatologiaService.listarTodas();
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las patologías', life: 3000 });
    } finally {
        cargando.value = false;
    }
};

const abrirCrear = () => {
    esEdicion.value = false;
    idEnEdicion.value = null;
    nombrePatologia.value = '';
    dialogoVisible.value = true;
};

const abrirEditar = (item: PatologiaResponseDTO) => {
    esEdicion.value = true;
    idEnEdicion.value = item.Id_Patologia;
    nombrePatologia.value = item.Nombre;
    dialogoVisible.value = true;
};

const guardar = async () => {
    if (!nombrePatologia.value.trim()) {
        toast.add({ severity: 'warn', summary: 'Atención', detail: 'El nombre es obligatorio', life: 3000 });
        return;
    }

    enviando.value = true;
    try {
        if (esEdicion.value && idEnEdicion.value) {
            await PatologiaService.modificar(idEnEdicion.value, { Nombre: nombrePatologia.value });
            toast.add({ severity: 'success', summary: 'Actualizado', detail: 'Patología modificada correctamente', life: 3000 });
        } else {
            await PatologiaService.crear({ Nombre: nombrePatologia.value });
            toast.add({ severity: 'success', summary: 'Creado', detail: 'Patología creada correctamente', life: 3000 });
        }
        
        dialogoVisible.value = false;
        await cargarPatologias(); 

    } catch (error: any) {
        const mensajeError = error.response?.data?.detail || 'Ocurrió un error al guardar';
        toast.add({ severity: 'error', summary: 'Error', detail: mensajeError, life: 3000 });
    } finally {
        enviando.value = false;
    }
};

const confirmarEliminar = (item: PatologiaResponseDTO) => {
    confirm.require({
        message: `¿Estás seguro de eliminar la patología "${item.Nombre}"?`,
        header: 'Confirmar Eliminación',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                await PatologiaService.eliminar(item.Id_Patologia);
                toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Patología eliminada', life: 3000 });
                patologias.value = patologias.value.filter(p => p.Id_Patologia !== item.Id_Patologia);
            } catch (error: any) {
                const mensajeError = error.response?.data?.detail || 'No se pudo eliminar (quizás está asignada a pacientes)';
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
                <i class="pi pi-heart me-2"></i>Gestión de Patologías
            </h4>
            <Button 
                label="Nueva Patología" 
                icon="pi pi-plus" 
                severity="primary" 
                rounded 
                @click="abrirCrear" 
            />
        </div>

        <DataTable 
            :value="patologias" 
            :loading="cargando" 
            paginator 
            :rows="10" 
            :rowsPerPageOptions="[5, 10, 20]"
            tableStyle="min-width: 50rem"
            class="p-datatable-sm"
        >
            <template #empty>No se encontraron patologías.</template>

            <Column field="Id_Patologia" header="ID" sortable style="width: 10%"></Column>
            
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
            :header="esEdicion ? 'Editar Patología' : 'Nueva Patología'" 
            :style="{ width: '400px' }"
        >
            <div class="d-flex flex-column gap-3 mt-2">
                <div>
                    <label for="nombre" class="fw-bold mb-1">Nombre</label>
                    <InputText 
                        id="nombre" 
                        v-model="nombrePatologia" 
                        class="w-100" 
                        placeholder="Ej: Diabetes, Hipertensión..." 
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