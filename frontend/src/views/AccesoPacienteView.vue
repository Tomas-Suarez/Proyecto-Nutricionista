<script setup lang="ts">
import { ref, onMounted, computed, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { useToast } from 'primevue/usetoast';

import { useAuthStore } from '../stores/authStores';
import type { PacienteResponseDTO } from '../types/dto/response/PacienteResponseDTO';

import DietaPdf from '../components/dieta/DietaPdf.vue';
import html2pdf from 'html2pdf.js';

import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Chart from 'primevue/chart';
import Select from 'primevue/select';
import { PublicPacienteService } from '../services/PublicPacienteService';

const route = useRoute();
const toast = useToast();
const authStore = useAuthStore();

const codigo = ref('');
const loading = ref(false);
const paciente = ref<PacienteResponseDTO | null>(null);
const tokenUrl = ref('');

const loadingPdf = ref(false);

const filtroDias = ref<number | null>(null);
const opcionesFiltro = ref([
    { label: 'Todo el historial', value: null },
    { label: 'Últimos 30 días', value: 30 },
    { label: 'Últimos 60 días', value: 60 },
    { label: 'Últimos 90 días', value: 90 }
]);

onMounted(() => {
    if (authStore.estaAutenticado) {
        window.location.href = '/app/dashboard';
        return;
    }
    tokenUrl.value = route.params.token as string;
});

const acceder = async () => {
    if (!codigo.value) return;
    loading.value = true;
    try {
        const data = await PublicPacienteService.acceder(tokenUrl.value, codigo.value);
        
        paciente.value = data; 
        
        const nombreCompleto = `${data.Nombre} ${data.Apellido}`
        toast.add({ severity: 'success', summary: 'Bienvenido', detail: `Hola ${nombreCompleto}`, life: 3000 });
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Código incorrecto o enlace inválido.', life: 3000 });
    } finally {
        loading.value = false;
    }
};

const getDownloadUrl = (ruta: string) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    return `${baseUrl.replace('/api', '')}${ruta}`;
};

const nombreMostrado = computed(() => {
    return paciente.value?.Nombre +" "+ paciente.value?.Apellido
});

const pesoActualMostrado = computed(() => {
    return (paciente.value as any)?.PesoActual || paciente.value?.PesoActual || '-';
});

const chartData = computed(() => {
    if (!paciente.value?.HistorialPeso || paciente.value.HistorialPeso.length === 0) {
        return { labels: [], datasets: [] };
    }

    let datos = [...paciente.value.HistorialPeso];

    if (filtroDias.value !== null) {
        const hoy = new Date();
        const limite = new Date();
        limite.setDate(hoy.getDate() - filtroDias.value);

        datos = datos.filter(p => {
            const fecha = new Date((p as any).Fecha_Pesaje || p.fecha_Pesaje);
            return fecha >= limite && fecha <= hoy; 
        });
    }

    datos.sort((a, b) => {
        const fechaA = (a as any).Fecha_Pesaje || a.fecha_Pesaje;
        const fechaB = (b as any).Fecha_Pesaje || b.fecha_Pesaje;
        return new Date(fechaA).getTime() - new Date(fechaB).getTime();
    });

    return {
        labels: datos.map(p => {
            const fecha = (p as any).Fecha_Pesaje || p.fecha_Pesaje;
            return new Date(fecha).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit' });
        }),
        datasets: [{
            label: 'Peso (kg)',
            data: datos.map(p => (p as any).Peso_Kg || p.peso_Kg),
            borderColor: '#695CFE',
            backgroundColor: 'rgba(105, 92, 254, 0.1)',
            pointBackgroundColor: '#695CFE',
            pointBorderColor: '#fff',
            fill: true,
            tension: 0.4
        }]
    };
});

const chartOptions = ref({
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: { display: false },
        tooltip: {
            backgroundColor: '#1d1b31',
            titleColor: '#ffffff',
            bodyColor: '#aeb5ce',
            borderColor: '#2d2b45',
            borderWidth: 1
        }
    },
    scales: {
        x: {
            grid: { display: false },
            ticks: { color: '#aeb5ce' }
        },
        y: {
            grid: { color: '#2d2b45' },
            ticks: { color: '#aeb5ce' },
            beginAtZero: false
        }
    }
});

const datosParaPdf = computed(() => {
    if (!paciente.value || !paciente.value.DietaActual) return null;

    const d = paciente.value.DietaActual as any;
    const p = paciente.value as any;

    const comidasMapeadas = (d.Comidas || d.comidas || []).map((c: any) => ({
        id_Comida: c.Id_Comida || c.id_Comida,
        nombreComida: c.NombreComida || c.nombreComida,
        nombreCategoria: c.NombreCategoria || c.nombreCategoria || 'Varios',
        cantidad: c.Cantidad || c.cantidad || '-',
        es_Permitido: c.Es_Permitido !== undefined ? c.Es_Permitido : c.es_Permitido,
        dia: c.Dia !== undefined ? c.Dia : c.dia,
        momento: c.Momento || c.momento
    }));

    return {
        nombre: d.Nombre || d.nombre,
        descripcion: d.Descripcion || d.descripcion || '',
        pacienteNombre: p.Nombre || p.nombre || '',
        peso: p.PesoActual || p.pesoActual || '-',
        talla: p.Altura_Cm || p.altura_Cm || '-',
        imc: p.Imc || p.imc || '-',
        objetivo: p.Objetivo?.Nombre || p.objetivo?.nombre || '',
        comidas: comidasMapeadas
    };
});

const generarPDF = async () => {
    loadingPdf.value = true;
    try {
        toast.add({ severity: 'info', summary: 'Generando', detail: 'Preparando tu plan...', life: 2000 });

        await nextTick();

        setTimeout(async () => {
            try {
                const elemento = document.getElementById('contenido-pdf');
                if (!elemento) throw new Error("Elemento PDF no encontrado");

                const nombrePlan = (paciente.value?.DietaActual as any)?.Nombre || paciente.value?.DietaActual?.nombre || 'Plan';
                const nombreArchivo = `Plan_${nombrePlan.replace(/[^a-z0-9]/gi, '_').toLowerCase()}.pdf`;

                const opciones = {
                    margin: 10,
                    filename: nombreArchivo,
                    image: { type: 'jpeg' as const, quality: 0.98 },
                    html2canvas: { scale: 2, useCORS: true, logging: false },
                    jsPDF: { unit: 'mm' as const, format: 'a4' as const, orientation: 'portrait' as const }
                };

                await html2pdf().set(opciones).from(elemento).save();
                toast.add({ severity: 'success', summary: 'Listo', detail: 'PDF descargado correctamente.', life: 3000 });
            } catch (err) {
                console.error(err);
                toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo generar el documento.' });
            } finally {
                loadingPdf.value = false;
            }
        }, 500);

    } catch (error) {
        console.error("Error al iniciar PDF:", error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo inicializar la descarga.' });
        loadingPdf.value = false;
    }
};

</script>

<template>
    <div class="min-vh-100 p-4 d-flex flex-column align-items-center dark-theme-scope main-wrapper">

        <div v-if="!paciente" class="d-flex justify-content-center align-items-center flex-grow-1 w-100">
            <div class="card shadow-lg border-0 rounded-4 p-5" style="width: 100%; max-width: 450px;">
                <div class="text-center mb-4">
                    <div class="mb-3">
                        <i class="pi pi-lock text-accent" style="font-size: 2.5rem;"></i>
                    </div>
                    <h4 class="fw-bold m-0 text-main">Acceso Paciente</h4>
                    <p class="text-muted small mt-2">Ingresa el código de 4 dígitos provisto por tu nutricionista.</p>
                </div>

                <div class="d-flex flex-column gap-3">
                    <span class="p-input-icon-left w-100">
                        <i class="pi pi-key text-muted" />
                        <InputText v-model="codigo" placeholder="Código (Ej: 7755)"
                            class="w-100 text-center fs-5 custom-input" :maxlength="4" />
                    </span>
                    <Button label="Ingresar" icon="pi pi-arrow-right" class="w-100 rounded-pill btn-accent"
                        @click="acceder" :loading="loading" />
                </div>
            </div>
        </div>

        <div v-else class="container fade-in py-3" style="max-width: 1100px;">

            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 class="fw-bold m-0 text-main">
                        <i class="pi pi-user me-2 text-accent"></i>Hola, {{ nombreMostrado }}
                    </h3>
                    <p class="text-muted m-0 small">Bienvenido a tu panel de seguimiento.</p>
                </div>
                <Button label="Salir" icon="pi pi-sign-out" severity="secondary" text rounded @click="paciente = null"
                    class="text-muted hover-text-main" />
            </div>

            <div class="row g-4">

                <div class="col-lg-8 d-flex flex-column gap-4">

                    <div class="card shadow-sm border-0 rounded-4 p-4 h-100">
                        
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <h5 class="fw-bold m-0 text-main"><i class="pi pi-chart-line text-accent me-2"></i>Evolución
                                de Peso</h5>
                            
                            <Select 
                                v-model="filtroDias" 
                                :options="opcionesFiltro" 
                                optionLabel="label" 
                                optionValue="value" 
                                placeholder="Filtrar periodo" 
                                class="p-inputtext-sm"
                            />
                        </div>

                        <div style="height: 300px;">
                            <Chart v-if="chartData.labels.length > 0" type="line"
                                :data="chartData" :options="chartOptions" class="h-100 w-100" />
                            <div v-else
                                class="h-100 d-flex flex-column justify-content-center align-items-center text-muted">
                                <i class="pi pi-chart-bar mb-2" style="font-size: 2rem;"></i>
                                <p v-if="paciente.HistorialPeso && paciente.HistorialPeso.length > 0">No hay registros en este periodo.</p>
                                <p v-else>Aún no hay registros de peso.</p>
                            </div>
                        </div>
                    </div>

                    <div class="card shadow-sm border-0 rounded-4 p-4">
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <h5 class="fw-bold m-0 text-main">
                                <i class="pi pi-apple text-success me-2"></i>Tu Plan Alimentario
                            </h5>
                        </div>

                        <div v-if="paciente.DietaActual">
                            <div
                                class="d-flex justify-content-between align-items-center p-3 border-0 rounded-3 file-item-hover mt-2">
                                <div class="d-flex align-items-center gap-3 overflow-hidden">
                                    <div class="p-2 rounded-circle bg-icon-doc">
                                        <i class="pi pi-file-pdf text-danger" style="font-size: 1.2rem;"></i>
                                    </div>
                                    <div class="d-flex flex-column">
                                        <span class="fw-bold text-main text-truncate">
                                            {{ (paciente.DietaActual as any)?.Nombre || paciente.DietaActual?.nombre }}
                                        </span>
                                        <span class="text-muted small">
                                            Actualizado: {{ new Date((paciente.DietaActual as any)?.Fecha_Inicio ||
                                                (paciente.DietaActual as any)?.fecha_Inicio || new
                                            Date()).toLocaleDateString('es-AR') }}
                                        </span>
                                    </div>
                                </div>

                                <Button icon="pi pi-download" text rounded class="text-muted hover-text-main"
                                    v-tooltip.top="'Descargar PDF'" @click="generarPDF" :loading="loadingPdf" />
                            </div>
                        </div>
                        <div v-else class="text-center text-muted p-4 bg-body-tertiary rounded-3 mt-2">
                            <i class="pi pi-info-circle me-2"></i> No tienes una dieta activa asignada.
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 d-flex flex-column gap-4">

                    <div class="card border-0 rounded-4 bg-primary-gradient text-white p-4 text-center shadow-sm">
                        <div class="mb-2 opacity-75 fw-bold text-uppercase small">Peso Actual</div>
                        <div class="display-3 fw-bold mb-0">
                            {{ pesoActualMostrado }}<span class="fs-4 fw-normal">kg</span>
                        </div>
                        <div class="mt-2 opacity-75 small">
                            <i class="pi pi-history me-1"></i> Último registro
                        </div>
                    </div>

                    <div class="card shadow-sm border-0 rounded-4 p-4 flex-grow-1">
                        <div class="d-flex align-items-center mb-3">
                            <h5 class="fw-bold m-0 text-main"><i
                                    class="pi pi-folder-open text-warning me-2"></i>Documentos</h5>
                        </div>

                        <div v-if="paciente.ArchivosNutricionista && paciente.ArchivosNutricionista.length > 0"
                            class="d-flex flex-column gap-2">
                            <div v-for="archivo in paciente.ArchivosNutricionista"
                                :key="(archivo as any).Id_Archivo || archivo.Id_Archivo"
                                class="d-flex justify-content-between align-items-center p-3 border-0 rounded-3 file-item-hover">
                                <div class="d-flex align-items-center gap-3 overflow-hidden">
                                    <div class="p-2 rounded-circle bg-icon-doc">
                                        <i class="pi pi-file-pdf text-danger"></i>
                                    </div>
                                    <span class="fw-bold text-truncate text-main">{{ (archivo as any).Nombre ||
                                        archivo.Nombre }}</span>
                                </div>
                                <a :href="getDownloadUrl((archivo as any).Url || archivo.Url)" target="_blank"
                                    class="text-decoration-none">
                                    <Button icon="pi pi-download" text rounded class="text-muted hover-text-main"
                                        v-tooltip.top="'Descargar'" />
                                </a>
                            </div>
                        </div>
                        <div v-else class="text-center text-muted py-5">
                            <i class="pi pi-folder mb-2 opacity-50" style="font-size: 1.5rem;"></i>
                            <p class="m-0 small">No hay documentos compartidos.</p>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <div style="position: absolute; left: -9999px; top: 0; width: 800px;">
            <DietaPdf :dieta="datosParaPdf" />
        </div>
    </div>
</template>

<style scoped>
.dark-theme-scope {
    --body-bg: #0c0b16;
    --card-bg: #1d1b31;
    --text-primary: #ffffff;
    --text-secondary: #aeb5ce;
    --border-color: #2d2b45;
    --accent-color: #695CFE;
}

.main-wrapper {
    background-color: var(--body-bg);
    color: var(--text-primary);
    font-family: 'Poppins', sans-serif;
}

.card {
    background-color: var(--card-bg) !important;
    border: 1px solid var(--border-color) !important;
    color: var(--text-primary) !important;
}

.text-main {
    color: var(--text-primary) !important;
}

.text-muted {
    color: var(--text-secondary) !important;
}

.text-accent {
    color: var(--accent-color) !important;
}

.btn-accent {
    background-color: var(--accent-color) !important;
    border: none;
}

.bg-primary-gradient {
    background: linear-gradient(135deg, var(--accent-color) 0%, #4b39ef 100%);
}

.hover-text-main:hover {
    color: var(--text-primary) !important;
}

.custom-input {
    background-color: var(--body-bg) !important;
    border: 1px solid var(--border-color) !important;
    color: var(--text-primary) !important;
}

.custom-input:focus {
    border-color: var(--accent-color) !important;
    box-shadow: 0 0 0 2px rgba(105, 92, 254, 0.2) !important;
}

.file-item-hover {
    background-color: var(--body-bg);
    transition: transform 0.2s;
}

.file-item-hover:hover {
    transform: translateX(5px);
    background-color: var(--border-color);
}

.bg-icon-doc {
    background-color: var(--body-bg);
}

.border-custom {
    border-bottom: 1px solid var(--border-color) !important;
}

.border-custom-color {
    border-color: var(--border-color) !important;
}

.bg-body-tertiary {
    background-color: rgba(255, 255, 255, 0.05) !important;
}

.bg-body-secondary {
    background-color: var(--body-bg) !important;
}

:deep(.custom-accordion .p-accordion-header-link) {
    background-color: transparent !important;
    border: none !important;
    border-bottom: 1px solid var(--border-color) !important;
    color: var(--text-primary) !important;
    padding: 1.25rem 0;
}

:deep(.custom-accordion .p-accordion-content) {
    background-color: transparent !important;
    border: none !important;
    color: var(--text-secondary) !important;
    padding-bottom: 1rem;
}

:deep(.p-accordion-tab-active > .p-accordion-header .p-accordion-header-link) {
    color: var(--accent-color) !important;
}

.fade-in {
    animation: fadeIn 0.6s ease-out forwards;
}

@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(20px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

:deep(.hoja-pdf) {
    background: white !important;
    color: black !important;
}
</style>