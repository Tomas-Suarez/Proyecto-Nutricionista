<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import { DietaService } from '../../services/DietaService';

import PacienteCard from '../PacienteCard.vue';
import DietaLista from './DietaLista.vue';
import DietaEditor from './DietaEditor.vue';

const props = defineProps<{ paciente: any }>();
const toast = useToast();
const confirm = useConfirm();

const pacienteIdSeguro = computed(() => {
    return Number(props.paciente.id_Paciente || props.paciente.Id_Paciente || props.paciente.id || 0);
});

const vistaActual = ref<'lista' | 'editor'>('lista');
const loading = ref(false);
const historialDietas = ref<any[]>([]);
const totalRegistros = ref(0);
const paginaActual = ref(1);
const tamanoPagina = ref(5);

const dietaIdSeleccionada = ref<number | null>(null);

const cargarHistorial = async () => {
    loading.value = true;
    try {
        if (!pacienteIdSeguro.value) return;

        const res: any = await DietaService.obtenerHistorialPaciente(pacienteIdSeguro.value, paginaActual.value, tamanoPagina.value);
        
        historialDietas.value = res.Items || res.items || [];
        totalRegistros.value = res.TotalCount || res.totalCount || 0;
    } catch (error) {
        console.error(error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar el historial.' });
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    cargarHistorial();
});

const onPageChange = (event: any) => {
    paginaActual.value = event.page + 1;
    cargarHistorial();
};

const irACrear = () => {
    dietaIdSeleccionada.value = null; 
    vistaActual.value = 'editor';
};

const irAEditar = (data: any) => {
    dietaIdSeleccionada.value = data.id_Dieta || data.Id_Dieta;
    vistaActual.value = 'editor';
};

const eliminarDieta = (data: any) => {
    const id = data.id_Dieta || data.Id_Dieta;
    confirm.require({
        message: `¿Borrar la dieta "${data.Nombre}"?`,
        header: 'Confirmar',
        icon: 'pi pi-info-circle',
        rejectLabel: 'Cancelar',
        acceptLabel: 'Eliminar',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                await DietaService.eliminar(id);
                toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Plan eliminado correctamente', life: 3000 });
                cargarHistorial();
            } catch (e) {
                toast.add({ severity: 'error', summary: 'Error al eliminar' });
            }
        }
    });
};

const cerrarEditor = () => {
    vistaActual.value = 'lista';
    dietaIdSeleccionada.value = null;
};

const onGuardadoExitoso = () => {
    toast.add({ severity: 'success', summary: 'Éxito', detail: 'Plan guardado correctamente.', life: 3000 });
    vistaActual.value = 'lista';
    paginaActual.value = 1;
    cargarHistorial();
};
</script>

<template>
    <div class="d-flex flex-column gap-3">
        <PacienteCard :paciente="paciente" />

        <DietaLista 
            v-if="vistaActual === 'lista'"
            :historial="historialDietas"
            :loading="loading"
            :totalRegistros="totalRegistros"
            :rows="tamanoPagina"
            @pageChange="onPageChange"
            @crear="irACrear"
            @editar="irAEditar"
            @eliminar="eliminarDieta"
        />

        <DietaEditor 
            v-if="vistaActual === 'editor'"
            :pacienteId="pacienteIdSeguro"
            :dietaIdEditar="dietaIdSeleccionada"
            @cerrar="cerrarEditor"
            @guardadoExitoso="onGuardadoExitoso"
        />
    </div>
</template>