import { defineStore } from 'pinia';
import { ref } from 'vue';
import { ComidaService } from '../services/ComidaService';

export const useAlimentosStore = defineStore('alimentos', () => {
    const catalogo = ref<any[]>([]);
    const loading = ref(false);
    const paginaActual = ref(1);
    const hayMasResultados = ref(true);
    
    const filtrosActuales = ref({
        idCategoria: undefined as number | undefined,
        busqueda: ''
    });

    const buscarDesdeCero = async (idCategoria?: number, texto?: string) => {
        catalogo.value = [];
        paginaActual.value = 1;
        hayMasResultados.value = true;
        filtrosActuales.value = { idCategoria, busqueda: texto || '' };
        
        await cargarPagina();
    };

    const cargarMas = async () => {
        if (!hayMasResultados.value || loading.value) return;
        paginaActual.value++;
        await cargarPagina();
    };

    const cargarPagina = async () => {
        loading.value = true;
        try {
            const respuesta: any = await ComidaService.obtenerTodas(
                paginaActual.value,
                20,
                filtrosActuales.value.idCategoria,
                filtrosActuales.value.busqueda
            );

            const itemsNuevos = respuesta.Items || respuesta.items || [];
            
            const itemsMapeados = itemsNuevos.map((item: any) => ({
                id_Comida: item.Id_Comida || item.id_Comida,
                nombre: item.Nombre || item.nombre,
                calorias: item.Calorias || item.calorias,
                porcion: item.Porcion || item.porcion,
                categoriaIds: item.Categorias ? item.Categorias.map((c: any) => c.Id_Categoria) : []
            }));

            if (paginaActual.value === 1) {
                catalogo.value = itemsMapeados;
            } else {
                catalogo.value.push(...itemsMapeados);
            }

            if (itemsMapeados.length < 20) {
                hayMasResultados.value = false;
            }

        } catch (error) {
            console.error(error);
        } finally {
            loading.value = false;
        }
    };

    return {
        catalogo,
        loading,
        hayMasResultados,
        buscarDesdeCero,
        cargarMas
    };
});