<script setup lang="ts">
import PerfilCard from '../components/configuracion/PerfilCard.vue';
import SeguridadCard from '../components/configuracion/SeguridadCard.vue';
</script>

<template>
    <div class="container-fluid p-4">
        <h2 class="mb-4 fw-bold text-light">Configuración</h2>

        <div class="row g-4">
            <div class="col-12">
                <PerfilCard />
            </div>

            <div class="col-12"> <SeguridadCard />
            </div>
        </div>
    </div>
</template>