<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm'; 
import { PesajeService } from '../../services/PesajeService';
import type { PesajeRequestDTO } from '../../types/dto/request/PesajeRequestDTO';

import PacienteCard from '../PacienteCard.vue'; 

import Chart from 'primevue/chart';
import Button from 'primevue/button';
import InputNumber from 'primevue/inputnumber';
import Calendar from 'primevue/calendar';
import Textarea from 'primevue/textarea';
import Card from 'primevue/card';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import Divider from 'primevue/divider';

const props = defineProps<{
    paciente: any; 
}>();

const toast = useToast();
const confirm = useConfirm(); 
const historial = ref<any[]>([]);
const loading = ref(false);
const enviando = ref(false);
const chartKey = ref(0);

const form = ref({
    peso: null as number | null,
    fecha: new Date(),
    grasa: null as number | null,
    musculo: null as number | null,
    nota: ''
});


const cargarHistorial = async () => {
    if (!props.paciente || !props.paciente.Id_Paciente) return;
    loading.value = true;
    try {
        const res = await PesajeService.obtenerHistorialPorPaciente(props.paciente.Id_Paciente, 1, 50);
        let datos = (res as any).Items || (res as any).items || (res as any).data || res;
        if (Array.isArray(datos)) {
            historial.value = datos;
            chartKey.value++; 
        } else {
            historial.value = [];
        }
    } catch (error) {
        console.error("Error historial:", error);
    } finally {
        loading.value = false;
    }
};

const confirmarEliminacion = (id: number) => {
    confirm.require({
        message: '¿Estás seguro de que deseas eliminar este registro?',
        header: 'Confirmación',
        icon: 'pi pi-exclamation-triangle',
        acceptLabel: 'Eliminar',
        rejectLabel: 'Cancelar',
        acceptClass: 'p-confirm-dialog-accept',
        rejectClass: 'p-button-text p-button-secondary',
        accept: async () => {
            try {
                await PesajeService.eliminar(id);
                toast.add({ severity: 'success', summary: 'Éxito', detail: 'Registro eliminado', life: 3000 });
                cargarHistorial();
            } catch (error) {
                toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar', life: 3000 });
            }
        }
    });
};

onMounted(() => cargarHistorial());
watch(() => props.paciente, () => cargarHistorial(), { deep: true });

const chartData = computed(() => {
    const datos = [...historial.value].reverse();
    return {
        labels: datos.map(p => {
            const f = p.Fecha_Pesaje || p.fecha_Pesaje;
            return f ? new Date(f).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit' }) : '';
        }),
        datasets: [
            {
                label: 'Peso (kg)',
                data: datos.map(p => p.Peso_Kg || p.peso_Kg || 0),
                fill: true,
                borderColor: '#10B981',
                backgroundColor: 'rgba(16, 185, 129, 0.1)',
                tension: 0.4,
                pointBackgroundColor: '#10B981',
                pointRadius: 4
            }
        ]
    };
});

const chartOptions = ref({
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { display: false } },
    scales: { 
        x: { grid: { display: false }, ticks: { color: '#9ca3af' } },
        y: { grid: { color: '#374151' }, ticks: { color: '#9ca3af' }, beginAtZero: false } 
    }
});

const guardarPesaje = async () => {
    if (!form.value.peso) return;
    enviando.value = true;
    try {
        const dto: PesajeRequestDTO = {
            Id_Paciente: props.paciente.Id_Paciente,
            Peso_Kg: form.value.peso,
            Fecha_Pesaje: form.value.fecha,
            Porcentaje_Grasa: form.value.grasa,
            Masa_Muscular_Kg: form.value.musculo,
            Nota: form.value.nota
        };
        await PesajeService.registrar(dto);
        toast.add({ severity: 'success', summary: 'Guardado', detail: 'Registro actualizado', life: 3000 });
        form.value = { peso: null, fecha: new Date(), grasa: null, musculo: null, nota: '' };
        cargarHistorial();
    } catch (error: any) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar', life: 3000 });
    } finally {
        enviando.value = false;
    }
};
</script>

<template>
    <div class="d-flex flex-column gap-3">
        
        <PacienteCard :paciente="paciente" />

        <div class="row g-3">
            <div class="col-lg-4">
                <Card class="shadow-sm border-0 h-100 bg-surface-50">
                    <template #content>
                        <div class="d-flex flex-column gap-3">
                            <h6 class="text-primary fw-bold mb-0"><i class="pi pi-plus-circle me-2"></i>Nuevo Registro</h6>
                            <Divider class="my-0"/>
                            <div>
                                <label class="small fw-bold mb-1">Fecha</label>
                                <Calendar v-model="form.fecha" showIcon dateFormat="dd/mm/yy" class="w-100 p-inputtext-sm" />
                            </div>
                            
                            <div class="row g-2">
                                <div class="col-12">
                                    <label class="small fw-bold text-success">Peso (kg)*</label>
                                    <InputNumber v-model="form.peso" :min="0" :maxFractionDigits="2" suffix=" kg" class="w-100 p-inputtext-sm" />
                                </div>
                                <div class="col-6">
                                    <label class="small fw-bold mb-1">Masa Muscular</label>
                                    <InputNumber v-model="form.musculo" :min="0" :maxFractionDigits="2" suffix=" kg" class="w-100 p-inputtext-sm" />
                                </div>
                                <div class="col-6">
                                    <label class="small fw-bold mb-1">% Grasa</label>
                                    <InputNumber v-model="form.grasa" :min="0" :max="100" suffix=" %" class="w-100 p-inputtext-sm" />
                                </div>
                            </div>

                            <div>
                                <label class="small fw-bold mb-1">Notas / Observaciones</label>
                                <Textarea v-model="form.nota" rows="5" class="w-100" placeholder="Escribe aquí los detalles..." style="resize: none;" />
                            </div>
                            <Button label="Guardar" icon="pi pi-save" class="w-100 mt-2" :loading="enviando" @click="guardarPesaje" />
                        </div>
                    </template>
                </Card>
            </div>

            <div class="col-lg-8">
                <Card class="shadow-sm border-0 h-100">
                    <template #content>
                        <div style="height: 250px;" class="mb-4">
                            <h6 class="text-primary fw-bold mb-0">EVOLUCIÓN DE PESO</h6>
                            <Chart type="line" :data="chartData" :options="chartOptions" :key="chartKey" class="h-100 w-100" />
                        </div>
                        
                        <Divider />
                        
                        <h6 class="text-primary fw-bold mb-0">HISTORIAL DETALLADO</h6>
                        
                        <DataTable :value="historial" :rows="4" paginator size="small" class="tiny-table">
                            <Column header="Fecha">
                                <template #body="{ data }">
                                    {{ new Date(data.Fecha_Pesaje || data.fecha_Pesaje).toLocaleDateString() }}
                                </template>
                            </Column>
                            <Column header="Peso">
                                <template #body="{ data }">
                                    <span class="fw-bold">{{ data.Peso_Kg || data.peso_Kg }} kg</span>
                                </template>
                            </Column>
                            
                            <Column header="Diferencia">
                                <template #body="{ data }">
                                    <div v-if="(data.DiferenciaPesoAnterior || data.diferenciaPesoAnterior) !== undefined">
                                        <Tag v-if="(data.DiferenciaPesoAnterior || data.diferenciaPesoAnterior) < 0" 
                                             severity="success" class="px-2">
                                            <i class="pi pi-arrow-down text-xs me-1"></i>
                                            {{ Math.abs(data.DiferenciaPesoAnterior || data.diferenciaPesoAnterior).toFixed(1) }}
                                        </Tag>
                                        <Tag v-else-if="(data.DiferenciaPesoAnterior || data.diferenciaPesoAnterior) > 0" 
                                             severity="danger" class="px-2">
                                            <i class="pi pi-arrow-up text-xs me-1"></i>
                                            {{ (data.DiferenciaPesoAnterior || data.diferenciaPesoAnterior).toFixed(1) }}
                                        </Tag>
                                        <span v-else class="text-muted small">=</span>
                                    </div>
                                    <span v-else class="text-muted small">-</span>
                                </template>
                            </Column>

                            <Column header="IMC" style="min-width: 100px;">
                                <template #body="{ data }">
                                    <div class="d-flex flex-column lh-1">
                                        <span class="fw-bold small">{{ (data.IMC || data.Imc || data.imc || 0).toFixed(1) }}</span>
                                    </div>
                                </template>
                            </Column>

                            <Column header="Nota">
                                <template #body="{ data }">
                                    <i v-if="data.Nota || data.nota" class="pi pi-comment text-primary" v-tooltip.top="data.Nota || data.nota"></i>
                                </template>
                            </Column>

                            <Column header="Acción" style="width: 50px">
                                <template #body="{ data }">
                                    <Button icon="pi pi-trash" severity="danger" text rounded @click="confirmarEliminacion(data.Id_Pesaje || data.id_Pesaje)" />
                                </template>
                            </Column>
                        </DataTable>
                    </template>
                </Card>
            </div>
        </div>
    </div>
</template>

<style scoped>
:deep(.tiny-table .p-datatable-thead > tr > th) {
    background: transparent;
    padding: 0.5rem;
    font-size: 0.75rem;
    text-transform: uppercase;
}
:deep(.tiny-table .p-datatable-tbody > tr > td) {
    padding: 0.5rem;
    font-size: 0.85rem;
}
</style>