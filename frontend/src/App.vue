<script setup lang="ts">
import { RouterView } from 'vue-router';
import ConfirmDialog from 'primevue/confirmdialog';
import Toast from 'primevue/toast';
</script>

<template>
  <ConfirmDialog></ConfirmDialog>
  <router-view />
  <Toast />
</template>

<style>
:root {
    
    --primary-color: #695CFE !important;
    --primary-color-text: #ffffff !important;
    
    --primary-50: #efeeff !important;
    --primary-100: #e0ddff !important;
    --primary-500: #695CFE !important;
    --primary-600: #5649d8 !important;
    --primary-700: #4a3ec0 !important;
    
    --p-primary-color: #695CFE !important;
    --p-primary-500: #695CFE !important;
}

body {
  margin: 0;
  padding: 0;
  font-family: 'Poppins', sans-serif;
  background-color: #0c0b16; 
  color: #fff;
  overflow-x: hidden;
}

.p-dialog, .p-confirm-dialog {
    background-color: #1d1b31 !important;
    border: 1px solid #2d2b45 !important;
    color: #fff !important;
    font-family: 'Poppins', sans-serif !important;
}

.p-dialog .p-dialog-header {
    background-color: #1d1b31 !important;
    color: #fff !important;
    border-bottom: 1px solid #2d2b45 !important;
}
.p-dialog .p-dialog-header-icon {
    color: #aeb5ce !important;
}
.p-dialog .p-dialog-header-icon:hover {
    background-color: rgba(255,255,255,0.05) !important;
    color: #fff !important;
}

.p-dialog .p-dialog-content {
    background-color: #1d1b31 !important;
    color: #fff !important;
    padding-bottom: 1rem !important;
}
.p-confirm-dialog-icon {
    color: #695CFE !important;
}

.p-dialog .p-dialog-footer {
    background-color: #1d1b31 !important;
    border-top: 1px solid #2d2b45 !important;
    padding: 1.5rem !important;
}

body .p-dialog .p-dialog-footer .p-button {
    box-shadow: none !important;
    outline: none !important;
}

body .p-dialog .p-confirm-dialog-accept {
    background: #695CFE !important;
    border: 1px solid #695CFE !important;
    color: #ffffff !important;
}
body .p-dialog .p-confirm-dialog-accept:not(:disabled):hover {
    background: #5649d8 !important; /* Violeta oscuro */
    border-color: #5649d8 !important;
    color: #ffffff !important;
}
body .p-dialog .p-confirm-dialog-accept:not(:disabled):active,
body .p-dialog .p-confirm-dialog-accept:focus-visible {
    background: #4a3ec0 !important;
    border-color: #4a3ec0 !important;
    box-shadow: 0 0 0 2px #1d1b31, 0 0 0 4px #695CFE !important;
}


body .p-dialog .p-confirm-dialog-reject {
    background-color: transparent !important;
    border: 1px solid #695CFE !important;
    color: #695CFE !important;
}
body .p-dialog .p-confirm-dialog-reject:not(:disabled):hover {
    background-color: rgba(105, 92, 254, 0.15) !important;
    color: #ffffff !important;
    border-color: #695CFE !important;
}
body .p-dialog .p-confirm-dialog-reject:focus-visible {
    box-shadow: 0 0 0 2px #1d1b31, 0 0 0 4px #695CFE !important;
}

.p-button-label {
    font-weight: 600 !important;
}
</style>