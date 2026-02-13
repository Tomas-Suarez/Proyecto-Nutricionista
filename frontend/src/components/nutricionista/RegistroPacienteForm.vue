<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useToast } from 'primevue/usetoast';
import { PacienteService } from '../../services/PacienteService';
import { ObjetivoService } from '../../services/ObjetivoService';
import { PatologiaService } from '../../services/PatologiaService';
import type { RegistroPacienteDTO } from '../../types/dto/request/RegistroPacienteDTO';

import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import InputMask from 'primevue/inputmask';
import Dropdown from 'primevue/dropdown';
import InputNumber from 'primevue/inputnumber';
import MultiSelect from 'primevue/multiselect';

const emit = defineEmits(['guardar', 'cancelar']);
const toast = useToast();
const enviando = ref(false);

const listaObjetivos = ref<any[]>([]);
const listaPatologias = ref<any[]>([]);

const generos = ref([
    { label: 'Masculino', value: 'Masculino' },
    { label: 'Femenino', value: 'Femenino' },
    { label: 'Otro', value: 'Otro' }
]);

const form = ref({
    nombre: '',
    apellido: '',
    email: '',
    dni: '',
    telefono: '',
    genero: 'Masculino',
    peso: null as number | null,
    altura: null as number | null,
    idObjetivo: null as number | null,
    idsPatologias: [] as number[]
});

onMounted(async () => {
    try {
        const [resObjetivos, resPatologias] = await Promise.all([
            ObjetivoService.listarTodas(), 
            PatologiaService.listarTodas()  
        ]);

        listaObjetivos.value = resObjetivos;
        listaPatologias.value = resPatologias;
    } catch (error) {
        console.error("Error cargando listas:", error);
        toast.add({ severity: 'error', summary: 'Error de Carga', detail: 'No se pudieron cargar objetivos o patologías', life: 3000 });
    }
});

const guardar = async () => {
    if (!form.value.nombre || !form.value.apellido || !form.value.email || !form.value.dni || !form.value.peso || !form.value.altura) {
        toast.add({ severity: 'warn', summary: 'Faltan datos', detail: 'Por favor completa los campos obligatorios (*)', life: 3000 });
        return;
    }

    enviando.value = true;

    try {
        const dto: RegistroPacienteDTO = {
            Nombre: form.value.nombre,
            Apellido: form.value.apellido,
            Email: form.value.email,
            Dni: form.value.dni,
            Telefono: form.value.telefono,
            Genero: form.value.genero,
            Peso_Inicial: form.value.peso,
            Altura_Cm: form.value.altura,
            IdObjetivo: form.value.idObjetivo,
            IdsPatologias: form.value.idsPatologias
        };

        await PacienteService.registrar(dto);

        emit('guardar'); 

    } catch (error: any) {
        const msg = error.response?.data?.detail || 'Error al registrar paciente';
        toast.add({ severity: 'error', summary: 'Error', detail: msg, life: 3000 });
    } finally {
        enviando.value = false;
    }
};
</script>

<template>
    <div class="d-flex flex-column gap-3 pt-2">
        
        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Nombre *</label>
                <InputText v-model="form.nombre" class="w-100" placeholder="Ej: Juan" />
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Apellido *</label>
                <InputText v-model="form.apellido" class="w-100" placeholder="Ej: Perez" />
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Email *</label>
                <InputText v-model="form.email" class="w-100" type="email" placeholder="cliente@email.com" />
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">DNI (Será su clave) *</label>
                <InputText v-model="form.dni" class="w-100" placeholder="Sin puntos" />
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Teléfono</label>
                <InputMask 
                    v-model="form.telefono" 
                    class="w-100" 
                    mask="999-9999999" 
                    placeholder="Ej: 266-4555555" 
                />
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Género *</label>
                <Dropdown 
                    v-model="form.genero" 
                    :options="generos" 
                    optionLabel="label" 
                    optionValue="value" 
                    class="w-100" 
                />
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Peso Inicial (kg) *</label>
                <InputNumber 
                    v-model="form.peso" 
                    class="w-100" 
                    suffix=" kg" 
                    :min="0" 
                    :maxFractionDigits="2" 
                    placeholder="0.00"
                />
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Altura (cm) *</label>
                <InputNumber 
                    v-model="form.altura" 
                    class="w-100" 
                    suffix=" cm" 
                    :min="0" 
                    :max="300" 
                    placeholder="170"
                />
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Objetivo</label>
                <Dropdown 
                    v-model="form.idObjetivo" 
                    :options="listaObjetivos" 
                    optionLabel="Nombre" 
                    optionValue="Id_Objetivo" 
                    placeholder="Seleccionar..."
                    class="w-100" 
                    showClear
                />
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Patologías</label>
                <MultiSelect 
                    v-model="form.idsPatologias" 
                    :options="listaPatologias" 
                    optionLabel="Nombre" 
                    optionValue="Id_Patologia" 
                    placeholder="Ninguna" 
                    display="chip"
                    class="w-100"
                    :maxSelectedLabels="2"
                />
            </div>
        </div>

        <div class="d-flex justify-content-end gap-2 mt-3 border-top pt-3">
            <Button label="Cancelar" text @click="$emit('cancelar')" severity="secondary" />
            <Button label="Registrar Paciente" icon="pi pi-check" :loading="enviando" @click="guardar" />
        </div>
    </div>
</template>