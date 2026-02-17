<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useConfirm } from 'primevue/useconfirm';
import { NutricionistaService } from '../../services/NutricionistaService';
import type { ArchivoResponseDTO } from '../../types/dto/response/ArchivoResponseDTO';
import type { SubirPdfRequestDTO } from '../../types/dto/request/SubirPdfRequestDTO';

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Tag from 'primevue/tag';

const toast = useToast();
const confirm = useConfirm();

const archivos = ref<ArchivoResponseDTO[]>([]);
const loading = ref(false);
const uploading = ref(false);
const fileInput = ref<HTMLInputElement | null>(null);

const MAX_ARCHIVOS = 3;
const puedeSubir = computed(() => archivos.value.length < MAX_ARCHIVOS);

const cargarArchivos = async () => {
    loading.value = true;
    try {
        archivos.value = await NutricionistaService.obtenerMisArchivos();
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar la lista de archivos.', life: 3000 });
    } finally {
        loading.value = false;
    }
};

const triggerUpload = () => {
    fileInput.value?.click();
};

const handleFileChange = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
        const file = target.files[0];
        
        if (file!.type !== 'application/pdf') {
            toast.add({ severity: 'warn', summary: 'Formato incorrecto', detail: 'Solo se permiten archivos PDF.', life: 3000 });
            return;
        }

        await subirArchivo(file!);
    }
    if (fileInput.value) fileInput.value.value = '';
};

const subirArchivo = async (file: File) => {
    uploading.value = true;
    try {
        const dto: SubirPdfRequestDTO = { archivo: file };
        const nuevoArchivo = await NutricionistaService.subirPdf(dto);
        archivos.value.push(nuevoArchivo);
        toast.add({ severity: 'success', summary: 'Éxito', detail: 'Documento subido correctamente.', life: 3000 });
    } catch (error: any) {
        const status = error.response?.status;
        if (status === 409) {
            toast.add({ severity: 'warn', summary: 'Límite alcanzado', detail: 'Ya tienes 3 archivos subidos.', life: 3000 });
        } else {
            toast.add({ severity: 'error', summary: 'Error', detail: 'Falló la subida del archivo.', life: 3000 });
        }
    } finally {
        uploading.value = false;
    }
};

const confirmarEliminacion = (id: number) => {
    confirm.require({
        message: '¿Seguro que deseas eliminar este documento público?',
        header: 'Confirmar Eliminación',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                await NutricionistaService.eliminarArchivo(id);
                archivos.value = archivos.value.filter(a => a.Id_Archivo !== id);
                toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Archivo eliminado.', life: 3000 });
            } catch (e) {
                toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar.', life: 3000 });
            }
        }
    });
};

const formatDate = (dateString: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString('es-AR', {
        day: '2-digit', month: '2-digit', year: 'numeric'
    });
};

const getDownloadUrl = (ruta: string) => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    return `${baseUrl.replace('/api', '')}${ruta}`;
};

onMounted(() => {
    cargarArchivos();
});
</script>

<template>
    <div>
        <input type="file" ref="fileInput" accept="application/pdf" class="d-none" @change="handleFileChange" />

        <div class="d-flex justify-content-between align-items-center mb-4">
            <h4 class="fw-bold text-primary m-0">
                <i class="pi pi-folder me-2"></i>Mis Documentos
                <Tag :value="`${archivos.length}/3`" :severity="puedeSubir ? 'info' : 'warning'" class="ms-2" rounded />
            </h4>
            
            <Button 
                label="Subir PDF" 
                icon="pi pi-upload" 
                rounded 
                :disabled="!puedeSubir"
                :loading="uploading"
                @click="triggerUpload" 
            />
        </div>

        <DataTable :value="archivos" :loading="loading" responsiveLayout="scroll" 
            class="p-datatable-sm" tableStyle="min-width: 50rem">
            
            <template #empty>
                <div class="text-center py-3">No has subido documentos aún.</div>
            </template>

            <Column field="Nombre" header="Nombre del Archivo">
                <template #body="slotProps">
                    <div class="d-flex align-items-center gap-2">
                        <i class="pi pi-file-pdf text-danger text-xl"></i>
                        <span class="fw-bold">{{ slotProps.data.Nombre }}</span>
                    </div>
                </template>
            </Column>

            <Column field="Fecha" header="Fecha de Subida">
                <template #body="slotProps">
                    {{ formatDate(slotProps.data.Fecha) }}
                </template>
            </Column>

            <Column header="Acciones" style="width: 15%; text-align: center">
                <template #body="slotProps">
                    <a :href="getDownloadUrl(slotProps.data.Url)" 
                       target="_blank" 
                       class="text-decoration-none me-2">
                        <Button icon="pi pi-download" text rounded severity="info" v-tooltip.top="'Descargar'" />
                    </a>

                    <Button icon="pi pi-trash" text rounded severity="danger" 
                            @click="confirmarEliminacion(slotProps.data.Id_Archivo)" 
                            v-tooltip.top="'Eliminar'" />
                </template>
            </Column>
        </DataTable>
    </div>
</template>

<style scoped>
.d-none { display: none; }

:deep(.p-datatable-header) {
    background: transparent;
    border: none;
}
</style>