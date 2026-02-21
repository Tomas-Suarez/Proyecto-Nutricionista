<script setup lang="ts">
import { computed } from 'vue';
import Card from 'primevue/card';
import Tag from 'primevue/tag';

const props = defineProps<{
    paciente: any;
}>();

const datosImc = computed(() => {
    const imc = props.paciente.Imc;

    if (!imc || imc === 0) {
        return { valor: '--', etiqueta: 'Sin datos', color: 'text-muted' };
    }
    
    let clasificacion = 'Normal';
    let color = 'text-success';

    if (imc < 18.5) { clasificacion = 'Bajo Peso'; color = 'text-info'; }
    else if (imc >= 25 && imc < 30) { clasificacion = 'Sobrepeso'; color = 'text-warning'; }
    else if (imc >= 30) { clasificacion = 'Obesidad'; color = 'text-danger'; }

    return { 
        valor: imc.toFixed(1),
        etiqueta: clasificacion, 
        color: color 
    };
});
</script>

<template>
    <Card class="shadow-sm border-0">
        <template #content>
            <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
                
                <div class="d-flex gap-3 align-items-center">
                    <div class="p-3 rounded-circle shadow-sm d-flex align-items-center justify-content-center bg-primary bg-opacity-10" style="width: 60px; height: 60px;">
                        <i class="pi pi-user fs-2 text-primary"></i>
                    </div>
                    <div>
                        <h4 class="m-0 fw-bold" style="color: #7e22ce !important;">
                            {{ paciente.Nombre }} {{ paciente.Apellido }}
                        </h4>
                        <div class="small fw-bold mb-1">
                            <span><i class="pi pi-id-card me-1"></i>DNI: {{ paciente.Dni || '--' }}</span>
                            <span class="text-capitalize ms-2"><i class="pi pi-user me-1"></i>{{ paciente.Genero || 'Sin género' }}</span>
                        </div>
                    </div>
                </div>

                <div class="d-flex flex-column align-items-center px-4 border-start border-end border-secondary border-opacity-25 mx-auto">
                    <span class="small fw-bold mb-1">IMC Actual</span>
                    <div class="d-flex align-items-baseline gap-2">
                        <span class="fs-3 fw-bold">{{ datosImc.valor }}</span>
                        <span :class="datosImc.color" class="fw-bold small px-2 py-1 rounded shadow-sm bg-primary bg-opacity-10">
                            {{ datosImc.etiqueta }}
                        </span>
                    </div>
                    <span class="tiny-text small fw-bold mb-1">Altura: {{ paciente.Altura_Cm }} cm</span>
                </div>

                <div class="d-flex flex-column align-items-end" style="min-width: 150px;">
                    <span class="small text-uppercase small fw-bold mb-1">Objetivo</span>
                    <div class="fs-5 fw-bold text-primary mb-1">
                        {{ paciente.Objetivo ? paciente.Objetivo.Nombre : 'Sin definir' }}
                    </div>
                    <div class="d-flex flex-wrap gap-1 justify-content-end mt-1">
                         <template v-if="paciente.Patologias && paciente.Patologias.length > 0">
                            <Tag v-for="pat in paciente.Patologias" :key="pat.Id_Patologia" :value="pat.Nombre" severity="warning" class="text-xs" rounded />
                        </template>
                        <template v-else><span class="badge bg-secondary text-white text-xs">Sano</span></template>
                    </div>
                </div>

            </div>
        </template>
    </Card>
</template>

<style scoped>
.tiny-text { font-size: 0.7rem; }
.text-xs { font-size: 0.75rem; }
</style>