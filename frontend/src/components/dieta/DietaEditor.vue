<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useToast } from 'primevue/usetoast';
import { useAlimentosStore } from '../../stores/alimentoStore';
import { DietaService } from '../../services/DietaService';
import { CategoriaService } from '../../services/CategoriaService';
import { EHorarioComida } from '../../types/enum/EHorarioComida';
import type { DietaRequestDTO } from '../../types/dto/request/DietaRequestDTO';
import type { DietaComidaRequestDTO } from '../../types/dto/request/DietaComidaRequestDTO';

import Card from 'primevue/card';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import ScrollPanel from 'primevue/scrollpanel';
import Badge from 'primevue/badge';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import Accordion from 'primevue/accordion';
import AccordionTab from 'primevue/accordiontab';
import TabView from 'primevue/tabview';
import TabPanel from 'primevue/tabpanel';
import Calendar from 'primevue/calendar'; 
import Divider from 'primevue/divider';
import Dropdown from 'primevue/dropdown';

const props = defineProps<{ 
    pacienteId: number, 
    dietaIdEditar: number | null 
}>();

const emit = defineEmits(['cerrar', 'guardadoExitoso']);
const toast = useToast();
const alimentosStore = useAlimentosStore();

const loading = ref(false);
const indicesAcordeon = ref<number[]>([]);
const categorias = ref<any[]>([]);
const planGrupos = ref<any[]>([]);
const ejemplosMenu = ref([
    { nombre: 'Ejemplo 1', comidas: { 'Desayuno': [], 'Media Mañana': [], 'Almuerzo': [], 'Merienda': [], 'Cena': [] } as Record<string, any[]> },
    { nombre: 'Ejemplo 2', comidas: { 'Desayuno': [], 'Media Mañana': [], 'Almuerzo': [], 'Merienda': [], 'Cena': [] } as Record<string, any[]> }
]);

const dietaEnEdicion = ref({
    id: 0,
    nombre: '',
    descripcion: '',
    fechaInicio: new Date(),
    fechaFin: new Date(new Date().setDate(new Date().getDate() + 30)),
    activa: true
});

const estadosOptions = [{ label: 'Activo', value: true }, { label: 'Inactivo', value: false }];
const momentosDia = ['Desayuno', 'Media Mañana', 'Almuerzo', 'Merienda', 'Cena'];
const indiceGrupoMenu = ref(0); 
const busquedaCatalogo = ref('');
const indiceGrupoActivo = ref(0);
const menuActivoIndex = ref(0); 

const mapMomentoToEnum = (momento: string): EHorarioComida => {
    switch (momento) {
        case 'Desayuno': return EHorarioComida.Desayuno;
        case 'Media Mañana': return EHorarioComida.MediaManana;
        case 'Almuerzo': return EHorarioComida.Almuerzo;
        case 'Merienda': return EHorarioComida.Merienda;
        case 'Cena': return EHorarioComida.Cena;
        default: return EHorarioComida.Clasificacion;
    }
};

const mapEnumToMomentoVisual = (momento: string) => (momento === 'MediaManana' ? 'Media Mañana' : momento);

const getEstiloMomento = (momento: string) => {
    switch (momento) {
        case 'Desayuno': return { color: '#fbbf24', border: 'border-top: 3px solid #fbbf24' };
        case 'Media Mañana': return { color: '#22d3ee', border: 'border-top: 3px solid #22d3ee' };
        case 'Almuerzo': return { color: '#f87171', border: 'border-top: 3px solid #f87171' };
        case 'Merienda': return { color: '#fb923c', border: 'border-top: 3px solid #fb923c' };
        case 'Cena': return { color: '#818cf8', border: 'border-top: 3px solid #818cf8' };
        default: return { color: '#fff', border: '' };
    }
};

const inicializarEditor = async () => {
    loading.value = true;
    try {
        const data: any = await CategoriaService.obtenerTodas();
        const listaCats = Array.isArray(data) ? data : (data.Items || data.items || []);
        categorias.value = listaCats;

        // Mapeo seguro con conversión a número
        planGrupos.value = listaCats.map((c: any) => ({
            id: Number(c.Id_Categoria || c.id_Categoria),
            nombre: c.Nombre || c.nombre,
            label: c.Nombre || c.nombre,
            icon: 'pi pi-tag',
            permitidos: [],
            evitar: []
        }));

        if (props.dietaIdEditar) {
            await cargarDietaExistente(props.dietaIdEditar);
        } else {
            dietaEnEdicion.value.nombre = 'Nuevo Plan Nutricional';
        }

        if (planGrupos.value.length > 0) {
            alimentosStore.buscarDesdeCero(planGrupos.value[0].id, '');
        }
    } catch (e) {
        console.error(e);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar el editor.' });
    } finally {
        loading.value = false;
    }
};

const cargarDietaExistente = async (id: number) => {
    const dietaCompleta: any = await DietaService.obtenerPorId(id);
    
    dietaEnEdicion.value = {
        id: dietaCompleta.id_Dieta || dietaCompleta.Id_Dieta,
        nombre: dietaCompleta.nombre || dietaCompleta.Nombre,
        descripcion: dietaCompleta.descripcion || dietaCompleta.Descripcion || '',
        fechaInicio: new Date(dietaCompleta.fecha_Inicio || dietaCompleta.Fecha_Inicio),
        fechaFin: new Date(dietaCompleta.fecha_Fin || dietaCompleta.Fecha_Fin),
        activa: dietaCompleta.activa !== undefined ? dietaCompleta.activa : (dietaCompleta.Activa || false)
    };

    const listaComidas = dietaCompleta.comidas || dietaCompleta.Comidas || [];
    
    if (listaComidas.length > 0) {
        listaComidas.forEach((dc: any) => {
            const idComida = dc.id_Comida || dc.Id_Comida;
            const nombreComida = dc.nombreComida || dc.NombreComida || 'Alimento';
            const cantidad = dc.cantidad || dc.Cantidad;
            const dia = Number(dc.dia !== undefined ? dc.dia : dc.Dia);
            const momento = dc.momento || dc.Momento;
            const esPermitido = dc.es_Permitido !== undefined ? dc.es_Permitido : dc.Es_Permitido;
            const nombreCategoria = dc.nombreCategoria || dc.NombreCategoria;

            const itemConDetalle = { id_Comida: idComida, nombre: nombreComida, detalle: cantidad };

            if (dia === 0) {
                const grupo = nombreCategoria 
                    ? planGrupos.value.find(g => g.nombre === nombreCategoria)
                    : planGrupos.value[0]; 

                if (grupo) {
                    if (esPermitido) grupo.permitidos.push(itemConDetalle);
                    else grupo.evitar.push(itemConDetalle);
                }
            } else {
                // MENÚ
                const indexMenu = dia - 1;
                if (ejemplosMenu.value[indexMenu]) {
                    const momentoVisual = mapEnumToMomentoVisual(momento);
                    if (ejemplosMenu.value[indexMenu].comidas[momentoVisual]) {
                        ejemplosMenu.value[indexMenu].comidas[momentoVisual].push(itemConDetalle);
                    }
                }
            }
        });
    }
};

onMounted(() => {
    inicializarEditor();
});

watch(indiceGrupoActivo, () => {
    busquedaCatalogo.value = '';
    const grupo = planGrupos.value[indiceGrupoActivo.value];
    if (grupo) alimentosStore.buscarDesdeCero(Number(grupo.id), '');
});

let timeoutBuscador: any = null;
watch(busquedaCatalogo, (nuevoTexto) => {
    clearTimeout(timeoutBuscador);
    const texto = nuevoTexto.trim();
    const grupo = planGrupos.value[indiceGrupoActivo.value];
    const idGrupo = grupo ? Number(grupo.id) : undefined;

    if (texto.length === 0) { 
        alimentosStore.buscarDesdeCero(idGrupo, ''); 
        return; 
    }
    
    if (texto.length < 4) return;
    
    timeoutBuscador = setTimeout(() => { 
        alimentosStore.buscarDesdeCero(idGrupo, texto); 
    }, 1500);
});

const grupoActual = computed(() => planGrupos.value[indiceGrupoActivo.value] || { permitidos: [], evitar: [], nombre: '', id: 0 });
const obtenerEstado = (id: number) => {
    if (grupoActual.value.permitidos.some((i: any) => i.id_Comida === id)) return 'permitido';
    if (grupoActual.value.evitar.some((i: any) => i.id_Comida === id)) return 'evitar';
    return null;
};
const permitidosParaMenu = computed(() => {
    const grupo = planGrupos.value[indiceGrupoMenu.value];
    return grupo ? grupo.permitidos : [];
});

const onDragStart = (evt: DragEvent, alimento: any, origen: string) => {
    if (evt.dataTransfer) {
        evt.dataTransfer.dropEffect = 'copy';
        evt.dataTransfer.effectAllowed = 'copy';
        evt.dataTransfer.setData('alimento', JSON.stringify(alimento));
        evt.dataTransfer.setData('origen', origen);
    }
};

const onDropClasificacion = (evt: DragEvent, esPermitido: boolean) => {
    const data = evt.dataTransfer?.getData('alimento');
    const origen = evt.dataTransfer?.getData('origen');

    if (data && origen === 'catalogo') {
        const alimento = JSON.parse(data);

        if (alimento.categoriaIds && alimento.categoriaIds.length > 0 && grupoActual.value.id) {
            const idsAlimento = alimento.categoriaIds.map(String);
            const idGrupo = String(grupoActual.value.id);
            if (!idsAlimento.includes(idGrupo)) {
                toast.add({ severity: 'warn', summary: 'Categoría incorrecta', detail: 'Este alimento no pertenece a esta categoría', life: 2000 });
                return;
            }
        }

        if (obtenerEstado(alimento.id_Comida)) return;

        const nuevoItem = { ...alimento, detalle: '' };
        if (esPermitido) grupoActual.value.permitidos.push(nuevoItem);
        else grupoActual.value.evitar.push(nuevoItem);
    }
};

const eliminarDeClasificacion = (esPermitido: boolean, index: number) => {
    if (esPermitido) grupoActual.value.permitidos.splice(index, 1);
    else grupoActual.value.evitar.splice(index, 1);
};

const onDropMenu = (evt: DragEvent, momento: string) => {
    const data = evt.dataTransfer?.getData('alimento');
    if (data) {
        const alimento = JSON.parse(data);
        const menuActual = ejemplosMenu.value[menuActivoIndex.value];
        if (menuActual && menuActual.comidas && menuActual.comidas[momento]) {
            menuActual.comidas[momento].push({
                tempId: Date.now(),
                ...alimento
            });
        }
    }
};

const eliminarDelMenu = (momento: string, index: number) => {
    const menuActual = ejemplosMenu.value[menuActivoIndex.value];
    if (menuActual && menuActual.comidas && menuActual.comidas[momento]) {
        menuActual.comidas[momento].splice(index, 1);
    }
};


const guardar = async () => {
    if (!dietaEnEdicion.value.nombre) {
        toast.add({ severity: 'warn', summary: 'Atención', detail: 'Falta el nombre del plan.' });
        return;
    }
    
    loading.value = true;
    const listaComidas: DietaComidaRequestDTO[] = [];

    planGrupos.value.forEach(grupo => {
        grupo.permitidos.forEach((item: any) => {
            listaComidas.push({ 
                id_Comida: item.id_Comida, 
                es_Permitido: true, 
                cantidad: item.detalle || '', 
                dia: 0, 
                momento: EHorarioComida.Clasificacion 
            });
        });
        grupo.evitar.forEach((item: any) => {
            listaComidas.push({ 
                id_Comida: item.id_Comida, 
                es_Permitido: false, 
                cantidad: item.detalle || '', 
                dia: 0, 
                momento: EHorarioComida.Clasificacion 
            });
        });
    });

    ejemplosMenu.value.forEach((menu, index) => {
        const diaBackend = index + 1;
        momentosDia.forEach(momentoVisual => {
            menu.comidas[momentoVisual]?.forEach((item: any) => {
                listaComidas.push({ 
                    id_Comida: item.id_Comida, 
                    es_Permitido: true, 
                    cantidad: item.detalle || '', 
                    dia: diaBackend, 
                    momento: mapMomentoToEnum(momentoVisual) 
                });
            });
        });
    });

    const dto: DietaRequestDTO = {
        id_Paciente: props.pacienteId,
        id_Nutricionista: 0, 
        nombre: dietaEnEdicion.value.nombre,
        descripcion: dietaEnEdicion.value.descripcion,
        fecha_Inicio: dietaEnEdicion.value.fechaInicio.toISOString(),
        fecha_Fin: dietaEnEdicion.value.fechaFin.toISOString(),
        activa: dietaEnEdicion.value.activa,
        comidas: listaComidas
    };

    try {
        if (dietaEnEdicion.value.id === 0) await DietaService.registrar(dto);
        else await DietaService.modificarDieta(dietaEnEdicion.value.id, dto);
        
        emit('guardadoExitoso');
    } catch (error) {
        console.error("Error al guardar:", error);
        toast.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar.' });
    } finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="d-flex flex-column gap-3">
        <Card class="shadow-sm border-0 bg-surface-50">
            <template #content>
                <div class="d-flex justify-content-between align-items-center">
                    <div class="d-flex align-items-center gap-2">
                        <Button icon="pi pi-arrow-left" text rounded @click="emit('cerrar')" />
                        <h6 class="fw-bold text-primary m-0">CONFIGURACIÓN</h6>
                    </div>
                    <Button label="Guardar" icon="pi pi-save" @click="guardar" :loading="loading" />
                </div>
                <Divider />
                <div class="section-dark">
                    <div class="row g-3">
                        <div class="col-md-6"><label class="small fw-bold">Nombre</label><InputText v-model="dietaEnEdicion.nombre" class="w-100" /></div>
                        <div class="col-md-2"><label class="small fw-bold">Desde</label><Calendar v-model="dietaEnEdicion.fechaInicio" showIcon dateFormat="dd/mm/yy" class="w-100" /></div>
                        <div class="col-md-2"><label class="small fw-bold">Hasta</label><Calendar v-model="dietaEnEdicion.fechaFin" showIcon dateFormat="dd/mm/yy" class="w-100" /></div>
                        <div class="col-md-2"><label class="small fw-bold">Estado</label><Dropdown v-model="dietaEnEdicion.activa" :options="estadosOptions" optionLabel="label" optionValue="value" class="w-100" /></div>
                        <div class="col-12"><label class="small fw-bold">Descripción / Notas</label><InputText v-model="dietaEnEdicion.descripcion" placeholder="Opcional..." class="w-100" /></div>
                    </div>
                </div>
            </template>
        </Card>

        <Card class="shadow-sm border-0">
            <template #content>
                <Accordion :multiple="true" v-model:activeIndex="indicesAcordeon">
                    <AccordionTab header="1. Selección de Alimentos">
                            <div class="mb-3 d-flex gap-2 overflow-auto pb-2">
                                <Button v-for="(grupo, index) in planGrupos" :key="grupo.id" :label="grupo.label"
                                    :icon="grupo.icon" :outlined="indiceGrupoActivo !== index"
                                    :severity="indiceGrupoActivo === index ? 'help' : 'secondary'" size="small"
                                    class="p-button-sm" @click="indiceGrupoActivo = index" />
                            </div>
                            <div class="section-dark">
                                <div class="row g-3">
                                    <div class="col-md-3">
                                        <div class="d-flex flex-column gap-2 h-100">
                                            <IconField iconPosition="left">
                                                <InputIcon class="pi pi-search" />
                                                <InputText v-model="busquedaCatalogo" placeholder="Buscar..."
                                                    class="w-100 p-inputtext-sm" />
                                            </IconField>

                                            <ScrollPanel style="height: 400px"
                                                class="border rounded p-2 bg-black-opacity position-relative">

                                                <div v-if="alimentosStore.loading"
                                                    class="d-flex justify-content-center align-items-center h-100 w-100 position-absolute top-0 start-0 bg-black-opacity"
                                                    style="z-index: 10;">
                                                    <i class="pi pi-spin pi-spinner text-primary"
                                                        style="font-size: 2rem;"></i>
                                                </div>

                                                <div class="d-flex flex-column gap-2">
                                                    <div v-if="alimentosStore.catalogo.length === 0 && !alimentosStore.loading"
                                                        class="text-center text-muted small mt-4">No se encontraron
                                                        alimentos.</div>

                                                    <div v-for="item in alimentosStore.catalogo" :key="item.id_Comida"
                                                        class="p-2 border rounded cursor-grab item-draggable bg-surface-card"
                                                        :class="{ 'opacity-50': obtenerEstado(item.id_Comida) }"
                                                        draggable="true"
                                                        @dragstart="onDragStart($event, item, 'catalogo')">
                                                        <div class="d-flex justify-content-between"><span
                                                                class="fw-bold small">{{
                                                                item.nombre }}</span><i
                                                                v-if="obtenerEstado(item.id_Comida)"
                                                                class="pi pi-check text-success text-xs"></i></div>
                                                        <small class="text-primary fw-bold mb-0">{{ item.porcion || '1 porción'
                                                            }} | {{
                                                            item.calorias }} kcal</small>
                                                    </div>

                                                    <div v-if="alimentosStore.hayMasResultados && alimentosStore.catalogo.length > 0"
                                                        class="text-center pt-2 pb-2">
                                                        <Button label="Cargar más" icon="pi pi-arrow-down" size="small"
                                                            text @click="alimentosStore.cargarMas()"
                                                            :loading="alimentosStore.loading" />
                                                    </div>
                                                </div>
                                            </ScrollPanel>
                                        </div>
                                    </div>

                                    <div class="col-md-9">
                                        <div class="row h-100">
                                            <div class="col-md-6">
                                                <div class="h-100 border border-success rounded p-2 d-flex flex-column gap-2"
                                                    style="background: rgba(34, 197, 94, 0.05)">
                                                    <div
                                                        class="d-flex justify-content-between text-success fw-bold small px-2">
                                                        <span>PERMITIDOS</span>
                                                        <Badge :value="grupoActual.permitidos.length"
                                                            severity="success"></Badge>
                                                    </div>
                                                    <div class="flex-grow-1 drop-zone" @dragover.prevent
                                                        @drop="onDropClasificacion($event, true)">
                                                        <ScrollPanel style="height: 360px">
                                                            <div class="d-flex flex-column gap-2 pe-2">
                                                                <div v-for="(item, i) in grupoActual.permitidos"
                                                                    :key="i"
                                                                    class="p-2 rounded bg-surface-card border-start border-4 border-success mb-2 shadow-sm">
                                                                    <div class="d-flex justify-content-between mb-1">
                                                                        <span class="small fw-bold">{{ item.nombre
                                                                            }}</span><i
                                                                            class="pi pi-times text-danger cursor-pointer"
                                                                            @click="eliminarDeClasificacion(true, i)"></i>
                                                                    </div>
                                                                    <InputText v-model="item.detalle"
                                                                        placeholder="Cant: Ej. 1 taza"
                                                                        class="w-100 p-inputtext-sm" />
                                                                </div>
                                                            </div>
                                                        </ScrollPanel>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="h-100 border border-danger rounded p-2 d-flex flex-column gap-2"
                                                    style="background: rgba(239, 68, 68, 0.05)">
                                                    <div
                                                        class="d-flex justify-content-between text-danger fw-bold small px-2">
                                                        <span>EVITAR</span>
                                                        <Badge :value="grupoActual.evitar.length" severity="danger">
                                                        </Badge>
                                                    </div>
                                                    <div class="flex-grow-1 drop-zone" @dragover.prevent
                                                        @drop="onDropClasificacion($event, false)">
                                                        <ScrollPanel style="height: 360px">
                                                            <div class="d-flex flex-column gap-2 pe-2">
                                                                <div v-for="(item, i) in grupoActual.evitar" :key="i"
                                                                    class="p-2 rounded bg-surface-card border-start border-4 border-danger mb-2 shadow-sm">
                                                                    <div class="d-flex justify-content-between mb-1">
                                                                        <span class="small fw-bold">{{ item.nombre
                                                                            }}</span><i
                                                                            class="pi pi-times text-secondary cursor-pointer"
                                                                            @click="eliminarDeClasificacion(false, i)"></i>
                                                                    </div>
                                                                    <InputText v-model="item.detalle"
                                                                        placeholder="Nota..."
                                                                        class="w-100 p-inputtext-sm" />
                                                                </div>
                                                            </div>
                                                        </ScrollPanel>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </AccordionTab>

                        <AccordionTab header="2. Armado de Menú - EJEMPLO">
                            <div class="mb-3 d-flex gap-2 overflow-auto pb-2">
                                <Button v-for="(grupo, index) in planGrupos" :key="grupo.id" :label="grupo.label"
                                    :icon="grupo.icon" :outlined="indiceGrupoMenu !== index"
                                    :severity="indiceGrupoMenu === index ? 'help' : 'secondary'" size="small"
                                    class="p-button-sm" @click="indiceGrupoMenu = index" />
                            </div>
                            <div class="section-dark">
                                <div class="row g-3">
                                    <div class="col-md-3">
                                        <div class="d-flex flex-column gap-2 h-100">
                                            <div class="small fw-bold text-success"><i
                                                    class="pi pi-check-circle me-1"></i>PERMITIDOS DE
                                                {{ planGrupos[indiceGrupoMenu]?.label }}</div>
                                            <ScrollPanel style="height: 400px" class="border rounded p-2 bg-black-opacity">
                                                <div v-if="permitidosParaMenu.length === 0"
                                                    class="text-center text-muted small p-4">No
                                                    hay permitidos en esta categoría.</div>
                                                <div class="d-flex flex-column gap-2">
                                                    <div v-for="item in permitidosParaMenu" :key="item.id_Comida"
                                                        class="p-2 border rounded cursor-grab item-draggable bg-surface-card border-start border-4 border-success"
                                                        draggable="true"
                                                        @dragstart="onDragStart($event, item, 'permitidos_menu')">
                                                        <div class="fw-bold small">{{ item.nombre }}</div>
                                                        <div class="text-xs text-info fst-italic" v-if="item.detalle">{{
                                                            item.detalle }}
                                                        </div>
                                                    </div>
                                                </div>
                                            </ScrollPanel>
                                        </div>
                                    </div>
                                    <div class="col-md-9">
                                        <TabView v-model:activeIndex="menuActivoIndex">
                                            <TabPanel v-for="(menu, idx) in ejemplosMenu" :key="idx"
                                                :header="menu.nombre" :value="idx">
                                                <div class="row g-3">
                                                    <div class="col-md-6" v-for="momento in momentosDia" :key="momento">
                                                        <div class="p-3 border rounded bg-black-opacity h-100 drop-zone"
                                                            :style="getEstiloMomento(momento).border" @dragover.prevent
                                                            @drop="onDropMenu($event, momento)">
                                                            <div
                                                                class="d-flex justify-content-between align-items-center mb-2 pb-2 border-bottom">
                                                                <span class="small fw-bold text-uppercase"
                                                                    :style="{ color: getEstiloMomento(momento).color }">{{
                                                                    momento
                                                                    }}</span>
                                                                <Badge :value="menu.comidas[momento]!.length"
                                                                    severity="info"></Badge>
                                                            </div>
                                                            <div class="d-flex flex-column gap-2 mb-2">
                                                                <div v-if="menu.comidas[momento]!.length === 0"
                                                                    class="text-muted text-xs text-center py-3 border border-dashed rounded opacity-50">
                                                                    Arrastra aquí</div>
                                                                <div v-for="(comida, cIdx) in menu.comidas[momento]"
                                                                    :key="cIdx"
                                                                    class="d-flex justify-content-between align-items-center p-2 rounded bg-surface-card border shadow-sm">
                                                                    <div class="d-flex flex-column lh-1">
                                                                        <span class="small fw-bold">{{ comida.nombre
                                                                            }}</span>
                                                                        <span v-if="comida.detalle"
                                                                            class="text-xs text-success mt-1 fw-bold"><i
                                                                                class="pi pi-angle-right me-1"></i>{{
                                                                                comida.detalle
                                                                                }}</span>
                                                                    </div>
                                                                    <i class="pi pi-times text-danger cursor-pointer text-xs"
                                                                        @click="eliminarDelMenu(momento, cIdx)"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </TabPanel>
                                        </TabView>
                                    </div>
                                </div>
                            </div>
                        </AccordionTab>
                </Accordion>
            </template>
        </Card>
    </div>
</template>

<style scoped>
.section-dark {
    background: rgba(255, 255, 255, 0.03);
    border-radius: 12px;
    padding: 1rem;
}

.bg-black-opacity {
    background-color: rgba(0, 0, 0, 0.2);
}

.bg-surface-card {
    background-color: #2d2b45;
    border: 1px solid #3f3d56;
}

.drop-zone {
    min-height: 150px;
    transition: background-color 0.2s;
}

.drop-zone:hover {
    background-color: rgba(255, 255, 255, 0.05);
}

.item-draggable {
    transition: transform 0.2s;
}

.item-draggable:hover {
    transform: translateX(5px);
    border-color: #6366f1 !important;
}

.cursor-grab {
    cursor: grab;
}

.text-xs {
    font-size: 0.75rem;
}

.text-info {
    color: #22d3ee !important;
}
</style>