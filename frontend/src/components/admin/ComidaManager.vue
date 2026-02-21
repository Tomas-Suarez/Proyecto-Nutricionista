<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import { ComidaService } from '../../services/ComidaService';
import { CategoriaService } from '../../services/CategoriaService';
import { useAuthStore } from '../../stores/authStores';
import type { ComidaResponseDTO } from '../../types/dto/response/ComidaResponseDTO';
import type { CategoriaResponseDTO } from '../../types/dto/response/CategoriaResponseDTO';

import DataTable, { type DataTablePageEvent } from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import MultiSelect from 'primevue/multiselect';
import Tag from 'primevue/tag';

const toast = useToast();
const confirm = useConfirm();
const authStore = useAuthStore();

const esAdmin = computed(() => authStore.rolUsuario?.toString() === 'Admin');

const comidas = ref<ComidaResponseDTO[]>([]);
const categoriasDisponibles = ref<CategoriaResponseDTO[]>([]);

const cargando = ref(true);
const totalRecords = ref(0);
const lazyParams = ref({
    page: 0,
    rows: 10
});

const terminoBusqueda = ref('');
const timeoutBusqueda = ref<any>(null);

const dialogoVisible = ref(false);
const enviando = ref(false);
const esEdicion = ref(false);
const idEnEdicion = ref<number | null>(null);

const nombre = ref('');
const porcion = ref('');
const calorias = ref<number | null>(null);
const proteinas = ref<number | null>(null);
const carbohidratos = ref<number | null>(null);
const grasas = ref<number | null>(null);
const categoriasSeleccionadas = ref<CategoriaResponseDTO[]>([]);

onMounted(async () => {
    await cargarCategorias();
    await cargarComidas();
});

const cargarComidas = async () => {
    cargando.value = true;
    try {
        const pageNumber = lazyParams.value.page + 1;
        const pageSize = lazyParams.value.rows;

        const response = await ComidaService.obtenerTodas(
            pageNumber, 
            pageSize, 
            undefined,
            terminoBusqueda.value
        );

        // @ts-ignore
        comidas.value = response.Content || response.Items || [];
        // @ts-ignore
        totalRecords.value = response.TotalElements || response.TotalCount || 0;

    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar las comidas', life: 3000 });
    } finally {
        cargando.value = false;
    }
};

const onInputBusqueda = () => {
    if (timeoutBusqueda.value) clearTimeout(timeoutBusqueda.value);
    
    timeoutBusqueda.value = setTimeout(() => {
        lazyParams.value.page = 0;
        cargarComidas();
    }, 1500);
};

const cargarCategorias = async () => {
    try {
        categoriasDisponibles.value = await CategoriaService.obtenerTodas();
    } catch (error) {
        console.error("Error categorías", error);
    }
};

const onPage = (event: DataTablePageEvent) => {
    lazyParams.value.page = event.page;
    lazyParams.value.rows = event.rows;
    cargarComidas();
};

const abrirCrear = () => {
    esEdicion.value = false;
    idEnEdicion.value = null;
    limpiarFormulario();
    dialogoVisible.value = true;
};

const abrirEditar = (item: ComidaResponseDTO) => {
    esEdicion.value = true;
    idEnEdicion.value = item.Id_Comida;

    nombre.value = item.Nombre;
    porcion.value = (item as any).Porcion || '';
    calorias.value = item.Calorias;
    proteinas.value = item.Proteinas;
    carbohidratos.value = item.Carbohidratos;
    grasas.value = item.Grasas;

    categoriasSeleccionadas.value = (item.Categorias as any) || [];

    dialogoVisible.value = true;
};

const limpiarFormulario = () => {
    nombre.value = '';
    porcion.value = '';
    calorias.value = null;
    proteinas.value = null;
    carbohidratos.value = null;
    grasas.value = null;
    categoriasSeleccionadas.value = [];
};

const guardar = async () => {
    if (!nombre.value.trim() || !porcion.value.trim() || calorias.value === null) {
        toast.add({ severity: 'warn', summary: 'Faltan datos', detail: 'Nombre, Porción y Calorías son obligatorios', life: 3000 });
        return;
    }
    if (categoriasSeleccionadas.value.length === 0) {
        toast.add({ severity: 'warn', summary: 'Categorías', detail: 'Selecciona al menos una categoría', life: 3000 });
        return;
    }
    if (categoriasSeleccionadas.value.length > 3) {
        toast.add({ severity: 'warn', summary: 'Categorías', detail: 'Máximo 3 categorías permitidas', life: 3000 });
        return;
    }

    enviando.value = true;

    const dto: any = {
        Nombre: nombre.value,
        Porcion: porcion.value,
        Calorias: calorias.value,
        Proteinas: proteinas.value || 0,
        Carbohidratos: carbohidratos.value || 0,
        Grasas: grasas.value || 0,
        Ids_Categorias: categoriasSeleccionadas.value.map(c => c.Id_Categoria)
    };

    try {
        if (esEdicion.value && idEnEdicion.value) {
            await ComidaService.modificar(idEnEdicion.value, dto);
            toast.add({ severity: 'success', summary: 'Actualizado', detail: 'Comida modificada', life: 3000 });
        } else {
            await ComidaService.crear(dto);
            toast.add({ severity: 'success', summary: 'Creado', detail: 'Comida creada', life: 3000 });
        }

        dialogoVisible.value = false;
        await cargarComidas();

    } catch (error: any) {
        if (error.response?.data?.errors) {
        } else {
            const msg = error.response?.data?.detail || 'Error al guardar';
            toast.add({ severity: 'error', summary: 'Error', detail: msg, life: 3000 });
        }
    } finally {
        enviando.value = false;
    }
};

const confirmarEliminar = (item: ComidaResponseDTO) => {
    confirm.require({
        message: `¿Eliminar "${item.Nombre}"?`,
        header: 'Confirmar',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                await ComidaService.eliminar(item.Id_Comida);
                toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Comida eliminada', life: 3000 });
                await cargarComidas();
            } catch (error: any) {
                toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar', life: 3000 });
            }
        }
    });
};
</script>

<template>
    <div>
        <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <h4 class="fw-bold text-primary m-0">
                <i class="pi pi-apple me-2"></i>Gestión de Comidas
            </h4>
            
            <div class="d-flex gap-2">
                <span class="p-input-icon-left">
                    <i class="pi pi-search" />
                    <InputText 
                        v-model="terminoBusqueda" 
                        placeholder="Buscar comida..." 
                        @input="onInputBusqueda" 
                        class="p-inputtext-sm" 
                        style="width: 250px;" 
                    />
                </span>
                <Button v-if="esAdmin" label="Nueva Comida" icon="pi pi-plus" rounded @click="abrirCrear" />
            </div>
        </div>

        <DataTable :value="comidas" :lazy="true" :paginator="true" :rows="lazyParams.rows" :totalRecords="totalRecords"
            :loading="cargando" @page="onPage" :rowsPerPageOptions="[5, 10, 20]" class="p-datatable-sm"
            tableStyle="min-width: 60rem">
            <template #empty>No hay comidas registradas.</template>

            <Column field="Nombre" header="Nombre"></Column>
            <Column field="Calorias" header="Kcal" style="width: 10%">
                <template #body="{ data }">
                    <span class="fw-bold">{{ data.Calorias }}</span>
                </template>
            </Column>

            <Column header="Macros" style="width: 20%">
                <template #body="{ data }">
                    <div class="small">P:{{ data.Proteinas }} C:{{ data.Carbohidratos }} G:{{ data.Grasas }}</div>
                </template>
            </Column>

            <Column header="Categorías" style="width: 25%">
                <template #body="{ data }">
                    <div class="d-flex gap-1 flex-wrap">
                        <Tag v-for="c in data.Categorias" :key="c.Id_Categoria" :value="c.Nombre" severity="info"
                            class="text-xs" />
                    </div>
                </template>
            </Column>

            <Column v-if="esAdmin" header="Acciones" style="width: 15%; text-align: center">
                <template #body="{ data }">
                    <Button icon="pi pi-pencil" text rounded severity="info" @click="abrirEditar(data)" />
                    <Button icon="pi pi-trash" text rounded severity="danger" @click="confirmarEliminar(data)" />
                </template>
            </Column>
        </DataTable>

        <Dialog v-model:visible="dialogoVisible" modal :header="esEdicion ? 'Editar' : 'Nueva'"
            :style="{ width: '500px' }">
            <div class="d-flex flex-column gap-3 mt-2">

                <div class="d-flex flex-column gap-1">
                    <label class="fw-bold">Nombre</label>
                    <InputText v-model="nombre" class="w-100" placeholder="Ej: Pechuga de Pollo" />
                </div>

                <div class="d-flex flex-column gap-1">
                    <label class="fw-bold">Porción de Referencia</label>
                    <InputText v-model="porcion" class="w-100" placeholder="Ej: 100g, 1 taza, 1 unidad..." />
                    <small class="text-muted">Indica la cantidad a la que corresponden las calorías.</small>
                </div>

                <div class="d-flex flex-column gap-1">
                    <label class="fw-bold">Categorías (Min 1 - Max 3)</label>
                    <MultiSelect v-model="categoriasSeleccionadas" :options="categoriasDisponibles" optionLabel="Nombre"
                        dataKey="Id_Categoria" placeholder="Seleccionar..." display="chip" :selectionLimit="3"
                        class="w-100" />
                </div>

                <div class="row g-2">
                    <div class="col-6"><label>Calorías</label>
                        <InputNumber v-model="calorias" class="w-100" />
                    </div>
                    <div class="col-6"><label>Proteínas</label>
                        <InputNumber v-model="proteinas" :maxFractionDigits="1" class="w-100" />
                    </div>
                    <div class="col-6"><label>Carbos</label>
                        <InputNumber v-model="carbohidratos" :maxFractionDigits="1" class="w-100" />
                    </div>
                    <div class="col-6"><label>Grasas</label>
                        <InputNumber v-model="grasas" :maxFractionDigits="1" class="w-100" />
                    </div>
                </div>
            </div>
            <template #footer>
                <Button label="Cancelar" text @click="dialogoVisible = false" />
                <Button label="Guardar" icon="pi pi-check" :loading="enviando" @click="guardar" />
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