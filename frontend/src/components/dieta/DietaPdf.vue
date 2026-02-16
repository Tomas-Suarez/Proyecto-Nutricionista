<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
    dieta: any
}>();

const getComidas = () => {
    if (!props.dieta || !props.dieta.comidas) return [];
    return props.dieta.comidas;
};

const permitidosPorCategoria = computed(() => {
    const lista = getComidas().filter((c: any) => c.dia === 0 && c.es_Permitido);
    return lista.reduce((acc: any, item: any) => {
        const cat = item.nombreCategoria || 'Varios';
        if (!acc[cat]) acc[cat] = [];
        acc[cat].push(item);
        return acc;
    }, {});
});

const prohibidosPorCategoria = computed(() => {
    const lista = getComidas().filter((c: any) => c.dia === 0 && !c.es_Permitido);
    return lista.reduce((acc: any, item: any) => {
        const cat = item.nombreCategoria || 'Varios';
        if (!acc[cat]) acc[cat] = [];
        acc[cat].push(item);
        return acc;
    }, {});
});

const diasDelMenu = computed(() => {
    const lista = getComidas();
    const dias = new Set(lista.filter((c: any) => c.dia > 0).map((c: any) => c.dia));
    return Array.from(dias).sort((a: any, b: any) => a - b);
});

const obtenerMenuPorDia = (dia: number) => {
    return getComidas().filter((c: any) => c.dia === dia);
};
</script>

<template>
    <div id="contenido-pdf" class="hoja-pdf">
        <div v-if="dieta && dieta.nombre">
            
            <div class="header-naranja">
                <span>“PLAN ALIMENTARIO”</span>
            </div>

            <div class="datos-paciente mt-4">
                <div class="dato-item">
                    <span class="icono">🍏</span> 
                    <strong>PACIENTE:</strong> {{ dieta.pacienteNombre }}
                </div>
                <div class="dato-item">
                    <span class="icono">🍏</span> 
                    <strong>PESO ACTUAL:</strong> {{ dieta.peso }} kg.
                </div>
                <div class="dato-item">
                    <span class="icono">🍏</span> 
                    <strong>TALLA:</strong> {{ dieta.talla }} cm.
                </div>
                <div class="dato-item">
                    <span class="icono">🍏</span> 
                    <strong>IMC:</strong> {{ dieta.imc }}
                </div>
                <div class="dato-item" v-if="dieta.objetivo">
                    <span class="icono">🍏</span> 
                    <strong>OBJETIVO:</strong> {{ dieta.objetivo }}
                </div>
                <div class="dato-item" v-if="dieta.descripcion">
                    <span class="icono">🍏</span> 
                    <strong>Diagnóstico:</strong> "{{ dieta.descripcion }}"
                </div>
            </div>

            <div class="caja-recomendacion">
                <span class="check">✓</span>
                Realizar 4 COMIDAS PRINCIPALES: 
                <span class="texto-naranja">DESAYUNO, ALMUERZO, MERIENDA Y CENA</span> 
                + COLACIONES <span class="texto-naranja">MEDIA MAÑANA O MEDIA TARDE</span>. 
                Para evitar la ansiedad.
            </div>

            <div class="subtitulo-seccion">
                <u>ALIMENTOS PERMITIDOS en 24 horas:</u>
            </div>
            <div class="titulo-grupos">GRUPOS DE ALIMENTOS:</div>

            <div class="lista-grupos mb-4">
                <div v-for="(items, categoria) in permitidosPorCategoria" :key="categoria" class="grupo-item">
                    <div class="renglon-alimento">
                        <span class="bullet-color bullet-permitido">➕</span>
                        <span class="categoria-label">{{ categoria }}:</span>
                        <span class="contenido-alimentos">
                            <span v-for="(item, index) in items" :key="item.id_Comida">
                                {{ item.nombreComida }}<span v-if="item.cantidad && item.cantidad !== '-'"> ({{ item.cantidad }})</span>
                                <span v-if="index < items.length - 1">, </span>
                            </span>.
                        </span>
                    </div>
                </div>
            </div>

            <div class="subtitulo-seccion texto-rojo">
                <u>ALIMENTOS A EVITAR / NO PERMITIDOS:</u>
            </div>

            <div class="lista-grupos border-rojo mb-4">
                <div v-for="(items, categoria) in prohibidosPorCategoria" :key="categoria" class="grupo-item">
                    <div class="renglon-alimento">
                        <span class="bullet-color bullet-prohibido">✖</span>
                        <span class="categoria-label label-prohibido">{{ categoria }}:</span>
                        <span class="contenido-alimentos">
                            <span v-for="(item, index) in items" :key="item.id_Comida">
                                {{ item.nombreComida }}<span v-if="item.cantidad && item.cantidad !== '-'"> ({{ item.cantidad }})</span>
                                <span v-if="index < items.length - 1">, </span>
                            </span>.
                        </span>
                    </div>
                </div>
                <p v-if="Object.keys(prohibidosPorCategoria).length === 0" class="text-muted text-center mt-1 small">
                    No se registraron restricciones específicas.
                </p>
            </div>

            <div class="seccion-recomendaciones">
                <div class="titulo-recomendaciones">RECOMENDACIONES:</div>
                <ul class="lista-recomendaciones">
                    <li><span class="check-li">✓</span> Respetar las 4 comidas y en lo posible incluir las 2 colaciones para poder evitar la ansiedad.</li>
                    <li><span class="check-li">✓</span> Consumir todos los grupos de alimentos respetando las cantidades adecuadas.</li>
                    <li><span class="check-li">✓</span> Comer con moderación, tranquilos, espaciados, y tratar de evitar conflictos durante las comidas.</li>
                    <li><span class="check-li">✓</span> Realizar actividad física. Evitar sedentarismo. Incluir caminatas con una duración de 30 minutos. Seguir con el entrenamiento.</li>
                    <li><span class="check-li">✓</span> Consumo moderado de sal.</li>
                    <li><span class="check-li">✓</span> Tomar abundante AGUA POTABLE. 2 -3 litros diarios.</li>
                    <li><span class="check-li">✓</span> Respetar las horas de descanso: dormir entre 7-8 hs diarios.</li>
                </ul>
            </div>

            <div v-if="diasDelMenu.length > 0">
                <div class="html2pdf__page-break"></div>
                
                <div class="header-naranja mt-4">
                    <span>EJEMPLOS DE MENÚ</span>
                </div>

                <div v-for="dia in diasDelMenu" :key="dia" class="dia-bloque mt-4">
                    <h4 class="titulo-dia">Día {{ dia }}</h4>
                    <table class="tabla-menu">
                        <thead>
                            <tr>
                                <th width="20%">Momento</th>
                                <th>Alimento</th>
                                <th width="20%">Cantidad</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="comida in obtenerMenuPorDia(Number(dia))" :key="comida.id_Comida">
                                <td class="fw-bold texto-naranja">{{ comida.momento }}</td>
                                <td>{{ comida.nombreComida }}</td>
                                <td>{{ comida.cantidad !== '-' ? comida.cantidad : '' }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

        </div>
    </div>
</template>

<style scoped>
.hoja-pdf {
    width: 100%;
    max-width: 800px;
    background: white;
    padding: 40px; 
    font-family: 'Calibri', 'Arial', sans-serif;
    color: #000;
    line-height: 1.3;
    font-size: 14px;
}

.header-naranja {
    background-color: #ED7D31;
    color: white;
    text-align: right;
    padding: 8px 15px;
    font-weight: bold;
    font-size: 16px;
    text-transform: uppercase;
    margin-bottom: 25px;
}

.datos-paciente { margin-bottom: 20px; }
.dato-item { margin-bottom: 6px; display: flex; align-items: center; }
.dato-item .icono { margin-right: 8px; font-size: 14px; }
.dato-item strong { margin-right: 5px; text-decoration: underline; }

.caja-recomendacion {
    border: 1px solid #555;
    padding: 10px 40px 10px 40px;
    margin: 20px 0;
    text-align: center;
    position: relative;
    font-weight: 500;
}
.caja-recomendacion .check {
    position: absolute;
    left: 15px;
    top: 50%;
    transform: translateY(-50%);
    font-size: 18px;
}
.texto-naranja { color: #ED7D31; font-weight: bold; }
.texto-rojo { color: #c00000; }

.subtitulo-seccion { text-align: center; font-weight: bold; margin-bottom: 5px; font-size: 15px; }
.titulo-grupos { text-align: center; font-weight: bold; font-size: 14px; margin-bottom: 15px; }

.lista-grupos {
    border: 1px solid #777;
    padding: 15px;
    background-color: #fff;
}
.border-rojo { border-color: #c00000; }

.grupo-item { margin-bottom: 12px; }
.renglon-alimento { display: block; }

.bullet-color {
    font-weight: bold;
    margin-right: 5px;
    font-family: monospace;
    font-size: 16px;
    vertical-align: middle;
}
.bullet-permitido { color: #007bff; }
.bullet-prohibido { color: #c00000; }

.categoria-label {
    background-color: #FFFF00;
    font-weight: bold;
    text-transform: uppercase;
    padding: 0 2px;
    margin-right: 5px;
    border: 1px solid #aaa;
    vertical-align: middle;
}
.label-prohibido { background-color: #fce4e4; }

/* ESTILOS RECOMENDACIONES FINALES */
.seccion-recomendaciones {
    margin-top: 20px;
    padding: 10px 5px;
}
.titulo-recomendaciones {
    font-weight: bold;
    text-decoration: underline;
    margin-bottom: 10px;
}
.lista-recomendaciones {
    list-style: none;
    padding: 0;
    margin: 0;
}
.lista-recomendaciones li {
    margin-bottom: 5px;
    display: flex;
    align-items: flex-start;
}
.check-li {
    color: #000;
    font-weight: bold;
    margin-right: 8px;
}

.contenido-alimentos { font-size: 14px; vertical-align: middle; }

.dia-bloque { border: 1px solid #ddd; padding: 10px; margin-bottom: 20px; }
.titulo-dia { color: #ED7D31; border-bottom: 2px solid #ED7D31; margin-bottom: 10px; }
.tabla-menu { width: 100%; border-collapse: collapse; font-size: 13px; }
.tabla-menu th, .tabla-menu td { border: 1px solid #ccc; padding: 6px; text-align: left; }
.tabla-menu th { background-color: #eee; }

.mb-4 { margin-bottom: 1.5rem; }
.mt-4 { margin-top: 1.5rem; }
.text-muted { color: #777; }
.text-center { text-align: center; }
</style>