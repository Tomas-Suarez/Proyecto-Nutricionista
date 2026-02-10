<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import { CategoriaService } from '../../services/CategoriaService';
import type { CategoriaResponseDTO } from '../../types/dto/response/CategoriaResponseDTO';

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';

const toast = useToast();
const confirm = useConfirm();

const categorias = ref<CategoriaResponseDTO[]>([]);
const cargando = ref(true);
const dialogoVisible = ref(false);
const enviando = ref(false);

const esEdicion = ref(false);
const idEnEdicion = ref<number | null>(null);
const nombreCategoria = ref('');

onMounted(async () => {
    await cargarCategorias();
});

const cargarCategorias = async () => {
    cargando.value = true;
    try {
        categorias.value = await CategoriaService.obtenerTodas();
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las categorías', life: 3000 });
    } finally {
        cargando.value = false;
    }
};

const abrirCrear = () => {
    esEdicion.value = false;
    idEnEdicion.value = null;
    nombreCategoria.value = '';
    dialogoVisible.value = true;
};

const abrirEditar = (categoria: CategoriaResponseDTO) => {
    esEdicion.value = true;
    idEnEdicion.value = categoria.Id_Categoria;
    nombreCategoria.value = categoria.Nombre;
    dialogoVisible.value = true;
};

const guardarCategoria = async () => {
    if (!nombreCategoria.value.trim()) {
        toast.add({ severity: 'warn', summary: 'Atención', detail: 'El nombre es obligatorio', life: 3000 });
        return;
    }

    enviando.value = true;
    try {
        if (esEdicion.value && idEnEdicion.value) {
            await CategoriaService.modificar(idEnEdicion.value, { nombre: nombreCategoria.value });
            toast.add({ severity: 'success', summary: 'Actualizado', detail: 'Categoría modificada correctamente', life: 3000 });
        } else {
            await CategoriaService.crear({ nombre: nombreCategoria.value });
            toast.add({ severity: 'success', summary: 'Creado', detail: 'Categoría creada correctamente', life: 3000 });
        }
        
        dialogoVisible.value = false;
        await cargarCategorias(); 

    } catch (error: any) {
        const mensajeError = error.response?.data?.detail || 'Ocurrió un error al guardar';
        toast.add({ severity: 'error', summary: 'Error', detail: mensajeError, life: 3000 });
    } finally {
        enviando.value = false;
    }
};

const confirmarEliminar = (categoria: CategoriaResponseDTO) => {
    confirm.require({
        message: `¿Estás seguro de eliminar la categoría "${categoria.Nombre}"?`,
        header: 'Confirmar Eliminación',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                await CategoriaService.eliminar(categoria.Id_Categoria);
                toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Categoría eliminada', life: 3000 });
                categorias.value = categorias.value.filter(c => c.Id_Categoria !== categoria.Id_Categoria);
            } catch (error: any) {
                const mensajeError = error.response?.data?.detail || 'No se pudo eliminar (quizás tiene comidas asociadas)';
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
                <i class="pi pi-tags me-2"></i>Gestión de Categorías
            </h4>
            <Button 
                label="Nueva Categoría" 
                icon="pi pi-plus" 
                severity="primary" 
                rounded 
                @click="abrirCrear" 
            />
        </div>

        <DataTable 
            :value="categorias" 
            :loading="cargando" 
            paginator 
            :rows="10" 
            :rowsPerPageOptions="[5, 10, 20]"
            tableStyle="min-width: 50rem"
            class="p-datatable-sm"
        >
            <template #empty>No se encontraron categorías.</template>

            <Column field="Id_Categoria" header="ID" sortable style="width: 10%"></Column>
            
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
            :header="esEdicion ? 'Editar Categoría' : 'Nueva Categoría'" 
            :style="{ width: '400px' }"
        >
            <div class="d-flex flex-column gap-3 mt-2">
                <div>
                    <label for="nombre" class="fw-bold mb-1">Nombre</label>
                    <InputText 
                        id="nombre" 
                        v-model="nombreCategoria" 
                        class="w-100" 
                        placeholder="Ej: Desayuno, Cena..." 
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
                    @click="guardarCategoria" 
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