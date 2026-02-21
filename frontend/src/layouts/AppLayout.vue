<template>
    <div class="d-flex w-100 min-vh-100 font-poppins main-wrapper" :class="{ 'dark-theme': isDarkMode }">

        <aside class="sidebar d-flex flex-column flex-shrink-0 p-3" :class="{ 'collapsed': isCollapsed }">

            <div class="logo-details d-flex align-items-center mb-4 px-2">
                <div class="logo_icon d-flex justify-content-center align-items-center rounded bg-primary-gradient">
                    <i class="pi pi-box text-white fs-4"></i>
                </div>
                <div class="logo_name ms-3 fw-bold fs-5 tracking-wide">NutriApp</div>
                <i class="pi pi-bars ms-auto cursor-pointer nav-toggle" id="btn" @click="toggleSidebar"></i>
            </div>

            <ul class="nav-list flex-grow-1 p-0 m-0 d-flex flex-column gap-2 list-unstyled overflow-auto">
                <li v-for="item in menuItems" :key="item.path">
                    <router-link :to="item.path"
                        class="d-flex align-items-center text-decoration-none rounded p-2 item-link"
                        active-class="active">
                        <i :class="item.icon" class="fs-5 min-w-icon text-center"></i>
                        <span class="links_name ms-3">{{ item.label }}</span>
                        <span class="tooltip shadow">{{ item.label }}</span>
                    </router-link>
                </li>
            </ul>

            <div class="user-area mt-auto pt-3 border-top border-secondary-subtle">

                <div class="d-flex align-items-center p-2 item-link mb-2 cursor-pointer rounded" @click="toggleTheme">
                    <i class="pi fs-5 min-w-icon text-center" :class="isDarkMode ? 'pi-sun' : 'pi-moon'"></i>
                    <span class="links_name ms-3">{{ isDarkMode ? 'Modo Claro' : 'Modo Oscuro' }}</span>
                    <span class="tooltip shadow">{{ isDarkMode ? 'Modo Claro' : 'Modo Oscuro' }}</span>
                </div>

                <router-link to="/app/configuracion"
                    class="d-flex align-items-center text-decoration-none rounded p-2 item-link mb-2"
                    active-class="active">
                    <i class="pi pi-cog fs-5 min-w-icon text-center"></i>
                    <span class="links_name ms-3">Configuración</span>
                    <span class="tooltip shadow">Configuración</span>
                </router-link>

                <div class="profile d-flex align-items-center p-2 rounded bg-profile">
                    <div class="profile_details d-flex align-items-center">

                        <div
                            class="user-avatar rounded d-flex justify-content-center align-items-center fw-bold overflow-hidden position-relative">
                            <img v-if="avatarSrc" :src="avatarSrc" alt="Perfil" class="w-100 h-100 object-fit-cover"
                                style="object-fit: cover;" />
                            <span v-else>
                                {{ authStore.iniciales }}
                            </span>
                        </div>

                        <div class="name_job ms-3">
                            <div class="name fw-bold fs-6 text-truncate" style="max-width: 130px;">
                                {{ authStore.nombreMostrar }}
                            </div>
                            <div class="job opacity-50 text-xs text-light">
                                {{ authStore.rolUsuario || 'Nutricionista' }}
                            </div>
                        </div>
                    </div>

                    <i class="pi pi-sign-out ms-auto cursor-pointer hover-danger text-white" @click="logout"></i>
                </div>
            </div>
        </aside>

        <div class="home-section flex-grow-1 d-flex flex-column h-100vh overflow-hidden">

            <header class="header-bg shadow-sm py-3 px-4 d-flex align-items-center">
                <Breadcrumb :home="home" :model="breadcrumbs" class="border-0 p-0 bg-transparent w-100">
                    <template #item="{ item }">
                        <router-link v-if="item.url" :to="item.url"
                            class="text-decoration-none d-flex align-items-center cursor-pointer">
                            <span v-if="item.icon" :class="item.icon" class="me-2 text-muted-custom"></span>
                            <span class="text-main fw-semibold" style="font-size: 0.95rem;">{{ item.label }}</span>
                        </router-link>
                        <span v-else class="text-muted-custom">{{ item.label }}</span>
                    </template>
                    <template #separator>
                        <i class="pi pi-angle-right text-muted-custom mx-2" style="font-size: 0.8rem"></i>
                    </template>
                </Breadcrumb>
            </header>

            <main class="p-4 overflow-auto flex-grow-1 content-bg">
                <router-view></router-view>
            </main>
        </div>

    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../stores/authStores';
import Breadcrumb from 'primevue/breadcrumb';
import 'primeicons/primeicons.css'

const route = useRoute();
const authStore = useAuthStore();
const isCollapsed = ref(false);

const backendUrl = import.meta.env.VITE_IMG_BASE_URL;

const avatarSrc = computed(() => {

    const path = authStore.usuario?.AvatarUrl;

    if (!path) return null;

    if (path.startsWith('http')) return path;


    return `${backendUrl}/${path.replace(/^\//, '')}`;
});

const isDarkMode = ref(localStorage.getItem('theme') === 'dark');
const toggleTheme = () => {
    isDarkMode.value = !isDarkMode.value;
    localStorage.setItem('theme', isDarkMode.value ? 'dark' : 'light');
};

const home = ref({ icon: 'pi pi-home', url: '/app/dashboard' });

const breadcrumbs = computed(() => {
    const path = route.path;
    const parts = path.split('/').filter(p => p && p !== 'app');

    const items = parts.map((part) => {
        let label = part.charAt(0).toUpperCase() + part.slice(1);
        let icon = '';
        if (part === 'configuracion') { label = 'Configuración'; icon = 'pi pi-cog'; }
        if (part === 'pacientes') icon = 'pi pi-users';
        if (part === 'dashboard') icon = 'pi pi-chart-bar';

        return { label, url: '', icon };
    });
    return items;
});


const menuItems = computed(() => {
    const rol = authStore.rolUsuario;

    if (rol?.toString() === "Admin") {
        return [
            { label: 'Inicio', icon: 'pi pi-home', path: '/app/dashboard' },
            { label: 'Categorías', icon: 'pi pi-tags', path: '/app/categorias' },
            { label: 'Comidas', icon: 'pi pi-apple', path: '/app/comidas' },
            { label: 'Objetivos', icon: 'pi pi-flag', path: '/app/objetivos' },
            { label: 'Patologías', icon: 'pi pi-heart', path: '/app/patologias' },
        ];
    }

    if (rol?.toString() === "Nutricionista") {
        return [
            { label: 'Inicio', icon: 'pi pi-home', path: '/app/dashboard' },
            { label: 'Pacientes', icon: 'pi pi-users', path: '/app/pacientes' },
            { label: 'Comidas', icon: 'pi pi-apple', path: '/app/comidas' },
            { label: 'Archivos', icon: 'pi pi-folder', path: '/app/archivos' },
        ];
    }
});

const toggleSidebar = () => { isCollapsed.value = !isCollapsed.value; };
const logout = () => { authStore.logout(); };
</script>

<style>
:root {
    --sidebar-bg: #11101d;
    --body-bg: #e4e9f7;
    --card-bg: #ffffff;
    --text-primary: #1d1b31;
    --text-secondary: #747474;
    --border-color: #e0e0e0;
    --accent-color: #695CFE;
    --sidebar-text: #fff;
    --sidebar-text-muted: #aeb5ce;
}

.dark-theme {
    --sidebar-bg: #11101d;
    --body-bg: #0c0b16;
    --card-bg: #1d1b31;
    --text-primary: #ffffff;
    --text-secondary: #aeb5ce;
    --border-color: #2d2b45;
    --sidebar-text: #fff;
}

.font-poppins {
    font-family: 'Poppins', sans-serif;
}

body {
    margin: 0;
    padding: 0;
    overflow: hidden;
}

.main-wrapper {
    background-color: var(--body-bg);
    color: var(--text-primary);
    transition: background-color 0.3s ease, color 0.3s ease;
}

.sidebar {
    width: 250px;
    background-color: var(--sidebar-bg);
    border-right: 1px solid var(--border-color);
    color: var(--sidebar-text);
    height: 100vh;
    z-index: 99;
    position: sticky;
    top: 0;
    transition: all 0.5s ease;
}

.nav-list::-webkit-scrollbar {
    display: none;
}

.nav-list {
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.sidebar .logo_name,
.sidebar .nav-toggle {
    color: #fff;
}

.logo_name {
    opacity: 1;
    transition: opacity 0.3s;
}

.item-link {
    color: var(--sidebar-text-muted);
    transition: 0.3s;
    height: 50px;
    display: flex;
    align-items: center;
    position: relative;
    white-space: nowrap;
}

.item-link:hover {
    background: #fff;
    color: #11101d;
}

.active {
    background: #fff;
    color: #11101d !important;
}

.dark-theme .item-link:hover {
    background: var(--body-bg);
    color: #fff;
}

.dark-theme .active {
    background: var(--card-bg);
    color: var(--accent-color) !important;
    border-left: 4px solid var(--accent-color);
}

.sidebar.collapsed {
    width: 78px;
}

.sidebar.collapsed .logo_name,
.sidebar.collapsed .links_name,
.sidebar.collapsed .profile_details {
    opacity: 0;
    pointer-events: none;
    display: none !important;
}

.sidebar.collapsed .logo-details {
    justify-content: center;
    padding: 0;
}

.sidebar.collapsed #btn {
    position: absolute;
    width: 100%;
    text-align: center;
    top: 50%;
    transform: translateY(-50%);
    right: 0;
}

.sidebar.collapsed .item-link {
    justify-content: center;
}

.sidebar.collapsed .item-link i {
    margin: 0;
}

.sidebar.collapsed .profile {
    justify-content: center;
    padding: 10px 0;
}

.sidebar.collapsed .pi-sign-out {
    margin: 0 !important;
}

.tooltip {
    position: absolute;
    top: 0;
    left: 122px;
    transform: translateY(-50%);
    border-radius: 6px;
    height: 35px;
    width: 122px;
    background: #fff;
    color: #11101d;
    line-height: 35px;
    text-align: center;
    box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
    transition: 0s;
    opacity: 0;
    pointer-events: none;
    display: none;
    font-size: 0.9rem;
    font-weight: 500;
    z-index: 1000;
}

.sidebar.collapsed .item-link:hover .tooltip {
    transition: all 0.5s ease;
    opacity: 1;
    top: 50%;
    display: block;
}

.header-bg {
    background-color: var(--card-bg);
    border-bottom: 1px solid var(--border-color);
}

.content-bg {
    background-color: var(--body-bg);
}

.text-main {
    color: var(--text-primary) !important;
}

.text-muted-custom {
    color: var(--text-secondary) !important;
}

.bg-profile {
    background: #1d1b31;
}

.profile .name {
    color: #fff;
}

.profile .job {
    color: var(--sidebar-text-muted);
}

.user-avatar {
    width: 45px;
    height: 45px;
    min-width: 45px;
    border-radius: 12px;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 1.2rem;
    font-weight: bold;
    background-color: var(--accent-color);
    color: #fff;
    overflow: hidden;
    position: relative;
}

.card {
    background-color: var(--card-bg) !important;
    border: 1px solid var(--border-color) !important;
    color: var(--text-primary) !important;
    transition: background-color 0.3s;
}

h1,
h2,
h3,
h4,
h5,
h6,
.text-dark {
    color: var(--text-primary) !important;
}

.text-muted {
    color: var(--text-secondary) !important;
}

.form-control,
.form-select {
    background-color: var(--body-bg) !important;
    border: 1px solid var(--border-color) !important;
    color: var(--text-primary) !important;
}

.form-control:focus {
    box-shadow: 0 0 0 0.25rem rgba(105, 92, 254, 0.25);
    border-color: var(--accent-color) !important;
    color: var(--text-primary) !important;
}

::placeholder {
    color: var(--text-secondary) !important;
    opacity: 0.7;
}

.table {
    --bs-table-bg: transparent;
    --bs-table-color: var(--text-primary);
    border-color: var(--border-color);
}

.table-hover tbody tr:hover {
    color: var(--text-primary);
    background-color: rgba(105, 92, 254, 0.1) !important;
}

.p-breadcrumb {
    background: transparent !important;
    padding: 0 !important;
}

.p-breadcrumb ul {
    margin: 0;
    padding: 0;
}
</style>