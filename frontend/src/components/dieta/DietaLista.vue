<script setup lang="ts">
import { ref, nextTick } from 'vue';
import Card from 'primevue/card';
import Button from 'primevue/button';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import Divider from 'primevue/divider';
import { useToast } from 'primevue/usetoast';
import { DietaService } from '../../services/DietaService';
import { PacienteService } from '../../services/PacienteService'; 
import html2pdf from 'html2pdf.js';
import DietaPdfDesign from './DietaPdf.vue'; 

const props = defineProps<{
    historial: any[],
    loading: boolean,
    totalRegistros: number,
    rows: number
}>();

const emit = defineEmits(['crear', 'editar', 'eliminar', 'pageChange']);
const toast = useToast();

const onPage = (event: any) => {
    emit('pageChange', event);
};

const dietaParaImprimir = ref<any>({ comidas: [] });
const loadingPdf = ref(false);

const generarPDF = async (dataResumida: any) => {
    loadingPdf.value = true;
    try {
        const idDieta = dataResumida.id_Dieta || dataResumida.Id_Dieta;
        
        const rawDieta: any = await DietaService.obtenerPorId(idDieta);
        
        const idPaciente = rawDieta.id_Paciente || rawDieta.Id_Paciente;
        const rawPaciente: any = await PacienteService.obtenerPorId(idPaciente);

        if (!rawDieta || !rawPaciente) throw new Error("Faltan datos.");

        const peso = rawPaciente.pesoActual || rawPaciente.PesoActual || 0;
        const talla = rawPaciente.altura_Cm || rawPaciente.Altura_Cm || 0;
        const imc = rawPaciente.imc || rawPaciente.Imc || 0;

        const datosFinales = {
            id_Dieta: rawDieta.id_Dieta || rawDieta.Id_Dieta,
            nombre: rawDieta.nombre || rawDieta.Nombre || 'Plan Personalizado',
            descripcion: rawDieta.descripcion || rawDieta.Descripcion || '',
            fechaInicio: rawDieta.fecha_Inicio || rawDieta.Fecha_Inicio,
            objetivo: rawPaciente.objetivo?.nombre || rawPaciente.Objetivo?.Nombre || '',
            
            pacienteNombre: `${rawPaciente.nombre || rawPaciente.Nombre} ${rawPaciente.apellido || rawPaciente.Apellido}`,
            peso: peso,
            talla: talla,
            imc: imc,

            comidas: (rawDieta.comidas || rawDieta.Comidas || []).map((c: any) => ({
                id_Comida: c.id_Comida || c.Id_Comida,
                nombreComida: c.nombreComida || c.NombreComida,
                cantidad: c.cantidad || c.Cantidad,
                dia: c.dia !== undefined ? c.dia : c.Dia,
                momento: c.momento || c.Momento,
                nombreCategoria: c.nombreCategoria || c.NombreCategoria,
                es_Permitido: c.es_Permitido !== undefined ? c.es_Permitido : c.Es_Permitido
            }))
        };

        dietaParaImprimir.value = datosFinales;

        await nextTick();
        
        setTimeout(async () => {
            try {
                const elemento = document.getElementById('contenido-pdf');
                if (!elemento) throw new Error("Elemento PDF no encontrado");

                const nombreArchivo = `Plan_${datosFinales.nombre.replace(/[^a-z0-9]/gi, '_').toLowerCase()}.pdf`;

                const opciones = {
                    margin: 10,
                    filename: nombreArchivo,
                    image: { type: 'jpeg', quality: 0.98 },
                    html2canvas: { scale: 2, useCORS: true, logging: false },
                    jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
                };

                toast.add({ severity: 'info', summary: 'Generando', detail: 'Espere un momento...', life: 2000 });
                await html2pdf().set(opciones).from(elemento).save();
                toast.add({ severity: 'success', summary: 'Listo', detail: 'PDF descargado', life: 3000 });
            } catch (err) {
                console.error(err);
            } finally {
                loadingPdf.value = false;
            }
        }, 500);

    } catch (error) {
        console.error("Error PDF:", error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo generar.' });
        loadingPdf.value = false;
    }
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
                :value="historial" :loading="loading" lazy paginator :rows="rows" :totalRecords="totalRegistros" 
                @page="onPage" class="tiny-table" sortField="activa" :sortOrder="-1"
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
                            <Button icon="pi pi-print" text rounded severity="secondary" @click="generarPDF(data)" :loading="loadingPdf && dietaParaImprimir?.id_Dieta === (data.id_Dieta || data.Id_Dieta)" />
                            <Button icon="pi pi-pencil" text rounded severity="info" @click="emit('editar', data)" />
                            <Button icon="pi pi-trash" text rounded severity="danger" @click="emit('eliminar', data)" />
                        </div>
                    </template>
                </Column>
            </DataTable>
        </template>
    </Card>

    <div style="position: absolute; left: -9999px; top: 0; width: 800px;">
        <DietaPdfDesign :dieta="dietaParaImprimir" />
    </div>
</template>

<style scoped>
:deep(.tiny-table .p-datatable-thead > tr > th) { background: transparent; padding: 0.5rem; font-size: 0.75rem; text-transform: uppercase; }
:deep(.tiny-table .p-datatable-tbody > tr > td) { padding: 0.5rem; font-size: 0.85rem; }
</style>