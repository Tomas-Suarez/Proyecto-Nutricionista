<script setup lang="ts">
import { ref, watch } from 'vue';
import { useToast } from 'primevue/usetoast';
import { PacienteService } from '../../services/PacienteService';
import { ObjetivoService } from '../../services/ObjetivoService';
import { PatologiaService } from '../../services/PatologiaService';
import type { PacienteResponseDTO } from '../../types/dto/response/PacienteResponseDTO';

import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import InputMask from 'primevue/inputmask';
import Dropdown from 'primevue/dropdown';
import InputNumber from 'primevue/inputnumber';
import MultiSelect from 'primevue/multiselect';

const props = defineProps<{
    pacienteEdicion?: PacienteResponseDTO | null;
}>();

const emit = defineEmits(['guardar', 'cancelar']);
const toast = useToast();
const enviando = ref(false);

const errores = ref<Record<string, string>>({});

const listaObjetivos = ref<any[]>([]);
const paginasCargadasObjetivos = new Set<number>();
const cargandoObjetivos = ref(false);
const inicializadoObjetivos = ref(false);

const listaPatologias = ref<any[]>([]);
const paginasCargadasPatologias = new Set<number>();
const cargandoPatologias = ref(false);
const inicializadoPatologias = ref(false);

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

const resetForm = () => {
    errores.value = {};
    form.value = {
        nombre: '', apellido: '', email: '', dni: '', telefono: '',
        genero: 'Masculino', peso: null, altura: null, 
        idObjetivo: null, idsPatologias: []
    };
};

const cargarDatosEdicion = () => {
    errores.value = {};
    if (props.pacienteEdicion) {
        form.value = {
            nombre: props.pacienteEdicion.Nombre || '',
            apellido: props.pacienteEdicion.Apellido || '',
            email: props.pacienteEdicion.Email || '',
            dni: props.pacienteEdicion.Dni || '',
            telefono: props.pacienteEdicion.Telefono || '',
            genero: props.pacienteEdicion.Genero || 'Masculino',
            peso: props.pacienteEdicion.Peso_Inicial ?? null,
            altura: props.pacienteEdicion.Altura_Cm ?? null,
            idObjetivo: props.pacienteEdicion.Objetivo?.Id_Objetivo || null,
            idsPatologias: props.pacienteEdicion.Patologias?.map(p => p.Id_Patologia) || []
        };
    } else {
        resetForm();
    }
};

const onLazyLoadObjetivos = async (event: any) => {
    const size = 10;
    const first = event.first; 
    const last = event.last;

    const paginaInicial = Math.floor(first / size) + 1;
    const paginaFinal = Math.floor((last - 1) / size) + 1; 

    for (let page = paginaInicial; page <= paginaFinal; page++) {
        if (paginasCargadasObjetivos.has(page)) continue;

        paginasCargadasObjetivos.add(page);
        cargandoObjetivos.value = true;
        
        try {
            const res: any = await ObjetivoService.obtenerPaginado(page, size);
            let _lista = [...listaObjetivos.value];
            
            if (!inicializadoObjetivos.value) {
                const total = res.TotalCount || res.totalCount || 0;
                _lista = Array.from({ length: total }).map(() => ({ Id_Objetivo: null, Nombre: 'Cargando...' }));
                inicializadoObjetivos.value = true;
            }

            const nuevos = res.Items || res.items || [];
            const startIndex = (page - 1) * size;
            
            for (let i = 0; i < nuevos.length; i++) {
                if (startIndex + i < _lista.length) {
                    _lista[startIndex + i] = nuevos[i]; 
                }
            }

            if (props.pacienteEdicion?.Objetivo) {
                const targetId = props.pacienteEdicion.Objetivo.Id_Objetivo;
                const indices = [];
                for(let i = 0; i < _lista.length; i++) {
                    if (_lista[i] && _lista[i].Id_Objetivo === targetId) indices.push(i);
                }
                
                if (indices.length > 1) {
                    for(let j = 1; j < indices.length; j++) {
                        _lista[indices[j]!] = { Id_Objetivo: null, Nombre: 'Cargando...' };
                    }
                }
                
                if (indices.length === 0 && _lista.length > 0) {
                    const emptyIndex = _lista.findIndex(x => x && x.Id_Objetivo === null);
                    if (emptyIndex !== -1) {
                        _lista[emptyIndex] = props.pacienteEdicion.Objetivo;
                    }
                }
            }

            listaObjetivos.value = _lista;

        } catch (error) {
            paginasCargadasObjetivos.delete(page); 
            console.error(error);
        } finally {
            cargandoObjetivos.value = false;
        }
    }
};

const onLazyLoadPatologias = async (event: any) => {
    const size = 10;
    const first = event.first;
    const last = event.last;

    const paginaInicial = Math.floor(first / size) + 1;
    const paginaFinal = Math.floor((last - 1) / size) + 1;

    for (let page = paginaInicial; page <= paginaFinal; page++) {
        if (paginasCargadasPatologias.has(page)) continue;

        paginasCargadasPatologias.add(page);
        cargandoPatologias.value = true;
        
        try {
            const res: any = await PatologiaService.obtenerPaginado(page, size);
            let _lista = [...listaPatologias.value];
            
            if (!inicializadoPatologias.value) {
                const total = res.TotalCount || res.totalCount || 0;
                _lista = Array.from({ length: total }).map(() => ({ Id_Patologia: null, Nombre: 'Cargando...' }));
                inicializadoPatologias.value = true;
            }

            const nuevos = res.Items || res.items || [];
            const startIndex = (page - 1) * size;
            
            for (let i = 0; i < nuevos.length; i++) {
                if (startIndex + i < _lista.length) {
                    _lista[startIndex + i] = nuevos[i];
                }
            }

            if (props.pacienteEdicion?.Patologias) {
                for (const patologia of props.pacienteEdicion.Patologias) {
                    const targetId = patologia.Id_Patologia;
                    const indices = [];
                    for(let i = 0; i < _lista.length; i++) {
                        if (_lista[i] && _lista[i].Id_Patologia === targetId) indices.push(i);
                    }
                    
                    if (indices.length > 1) {
                        for(let j = 1; j < indices.length; j++) {
                            _lista[indices[j]!] = { Id_Patologia: null, Nombre: 'Cargando...' };
                        }
                    }
                    
                    if (indices.length === 0 && _lista.length > 0) {
                        const emptyIndex = _lista.findIndex(x => x && x.Id_Patologia === null);
                        if (emptyIndex !== -1) {
                            _lista[emptyIndex] = patologia;
                        }
                    }
                }
            }

            listaPatologias.value = _lista;

        } catch (error) {
            paginasCargadasPatologias.delete(page);
            console.error(error);
        } finally {
            cargandoPatologias.value = false;
        }
    }
};

watch(() => props.pacienteEdicion, () => {
    listaObjetivos.value = [];
    paginasCargadasObjetivos.clear();
    inicializadoObjetivos.value = false;

    listaPatologias.value = [];
    paginasCargadasPatologias.clear();
    inicializadoPatologias.value = false;

    cargarDatosEdicion();

    onLazyLoadObjetivos({ first: 0, last: 10 });
    onLazyLoadPatologias({ first: 0, last: 10 });
}, { deep: true, immediate: true });

const guardar = async () => {
    if (!form.value.nombre || !form.value.apellido || !form.value.dni || !form.value.peso || !form.value.altura) {
        toast.add({ severity: 'warn', summary: 'Faltan datos', detail: 'Completa los campos obligatorios (*)', life: 3000 });
        return;
    }

    enviando.value = true;
    errores.value = {};

    try {
        const dto: any = {
            Nombre: form.value.nombre,
            Apellido: form.value.apellido,
            Email: form.value.email,
            Dni: form.value.dni,
            Telefono: form.value.telefono,
            Genero: form.value.genero,
            Peso_Inicial: form.value.peso,
            Altura_Cm: form.value.altura,
            Id_Objetivo: form.value.idObjetivo,
            IdsPatologias: form.value.idsPatologias
        };

        if (props.pacienteEdicion) {
            await PacienteService.modificarPaciente(props.pacienteEdicion.Id_Paciente, dto);
            toast.add({ severity: 'success', summary: 'Éxito', detail: 'Paciente actualizado', life: 3000 });
        } else {
            await PacienteService.registrar(dto);
            toast.add({ severity: 'success', summary: 'Éxito', detail: 'Paciente registrado', life: 3000 });
        }

        emit('guardar'); 
    } catch (error: any) {
        if (error.response?.status === 400 && error.response.data?.errors) {
            const erroresBackend = error.response.data.errors;
            for (const key in erroresBackend) {
                errores.value[key.toLowerCase()] = erroresBackend[key][0];
            }
            toast.add({ severity: 'error', summary: 'Error de validación', detail: 'Por favor, revisa los campos en rojo.', life: 4000 });
        } else {
            const msg = error.response?.data?.detail || error.response?.data?.message || 'Error en la solicitud';
            toast.add({ severity: 'error', summary: 'Error', detail: msg, life: 3000 });
        }
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
                <InputText v-model="form.nombre" class="w-100" :class="{ 'p-invalid': errores.nombre }" />
                <small class="text-danger d-block mt-1" v-if="errores.nombre">{{ errores.nombre }}</small>
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Apellido *</label>
                <InputText v-model="form.apellido" class="w-100" :class="{ 'p-invalid': errores.apellido }" />
                <small class="text-danger d-block mt-1" v-if="errores.apellido">{{ errores.apellido }}</small>
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Email</label>
                <InputText v-model="form.email" class="w-100" type="email" :class="{ 'p-invalid': errores.email }" />
                <small class="text-danger d-block mt-1" v-if="errores.email">{{ errores.email }}</small>
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">DNI *</label>
                <InputText v-model="form.dni" class="w-100" :class="{ 'p-invalid': errores.dni }" />
                <small class="text-danger d-block mt-1" v-if="errores.dni">{{ errores.dni }}</small>
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Teléfono</label>
                <InputMask v-model="form.telefono" class="w-100" mask="999-9999999" :class="{ 'p-invalid': errores.telefono }" />
                <small class="text-danger d-block mt-1" v-if="errores.telefono">{{ errores.telefono }}</small>
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Género *</label>
                <Dropdown v-model="form.genero" :options="generos" optionLabel="label" optionValue="value" class="w-100" :class="{ 'p-invalid': errores.genero }" />
                <small class="text-danger d-block mt-1" v-if="errores.genero">{{ errores.genero }}</small>
            </div>
        </div>

        <div class="row g-3">
            <div class="col-6">
                <label class="fw-bold mb-1">Peso Inicial (kg) *</label>
                <InputNumber v-model="form.peso" class="w-100" suffix=" kg" :min="0" :maxFractionDigits="2" :class="{ 'p-invalid': errores.peso_inicial }" />
                <small class="text-danger d-block mt-1" v-if="errores.peso_inicial">{{ errores.peso_inicial }}</small>
            </div>
            <div class="col-6">
                <label class="fw-bold mb-1">Altura (cm) *</label>
                <InputNumber v-model="form.altura" class="w-100" suffix=" cm" :min="0" :max="300" :class="{ 'p-invalid': errores.altura_cm }" />
                <small class="text-danger d-block mt-1" v-if="errores.altura_cm">{{ errores.altura_cm }}</small>
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
                    scrollHeight="200px" 
                    :class="{ 'p-invalid': errores.id_objetivo }"
                    :virtualScrollerOptions="{ 
                        lazy: true, 
                        onLazyLoad: onLazyLoadObjetivos, 
                        itemSize: 38, 
                        showLoader: true, 
                        loading: cargandoObjetivos 
                    }"
                />
                <small class="text-danger d-block mt-1" v-if="errores.id_objetivo">{{ errores.id_objetivo }}</small>
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
                    scrollHeight="200px"
                    :class="{ 'p-invalid': errores.idspatologias }"
                    :virtualScrollerOptions="{ 
                        lazy: true, 
                        onLazyLoad: onLazyLoadPatologias, 
                        itemSize: 38, 
                        showLoader: true, 
                        loading: cargandoPatologias 
                    }"
                />
                <small class="text-danger d-block mt-1" v-if="errores.idspatologias">{{ errores.idspatologias }}</small>
            </div>
        </div>

        <div class="d-flex justify-content-end gap-2 mt-3 border-top pt-3">
            <Button label="Cancelar" text @click="$emit('cancelar')" severity="secondary" />
            <Button 
                :label="pacienteEdicion ? 'Actualizar Paciente' : 'Registrar Paciente'" 
                :icon="pacienteEdicion ? 'pi pi-refresh' : 'pi pi-check'" 
                :loading="enviando" 
                @click="guardar" 
            />
        </div>
    </div>
</template>