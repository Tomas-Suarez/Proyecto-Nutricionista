<script setup lang="ts">
import PerfilCard from '../components/configuracion/PerfilCard.vue';
import SeguridadCard from '../components/configuracion/SeguridadCard.vue';
</script>

<template>
    <div class="container-fluid p-4 config-scroll">
        <h2 class="mb-4 fw-bold text-light">Configuración</h2>

        <div class="row g-4">
            <div class="col-12">
                <PerfilCard />
            </div>

            <div class="col-12"> 
                <SeguridadCard />
            </div>
        </div>
    </div>
</template>

<style scoped>
.config-scroll {
    max-height: calc(100vh - 80px); 
    overflow-y: auto;
    overflow-x: hidden;
    padding-right: 10px;
}

.config-scroll::-webkit-scrollbar {
    width: 8px;
}

.config-scroll::-webkit-scrollbar-track {
    background: transparent;
}

.config-scroll::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.2);
    border-radius: 10px;
}

.config-scroll::-webkit-scrollbar-thumb:hover {
    background: rgba(255, 255, 255, 0.4);
}
</style>