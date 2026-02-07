import { createApp } from 'vue';
import { createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';
import App from './App.vue';
import router from './router/index';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap'; 

import './style.css';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});

app.mount('#app');