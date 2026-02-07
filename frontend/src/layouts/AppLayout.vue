<template>
  <div class="d-flex w-100 min-vh-100 font-poppins bg-body-tertiary">
    
    <aside class="sidebar d-flex flex-column flex-shrink-0 p-3 transition-all" :class="{ 'collapsed': isCollapsed }">
      
      <div class="logo-details d-flex align-items-center mb-4 px-2">
        <div class="logo_icon d-flex justify-content-center align-items-center rounded bg-primary-gradient">
            <i class="pi pi-box text-white fs-4"></i>
        </div>
        <div class="logo_name ms-3 text-white fw-bold fs-5 tracking-wide">NutriApp</div>
        <i class="pi pi-bars ms-auto text-white cursor-pointer" id="btn" @click="toggleSidebar"></i>
      </div>

      <ul class="nav-list flex-grow-1 p-0 m-0 d-flex flex-column gap-2 list-unstyled overflow-auto">
        <li v-for="item in menuItems" :key="item.path">
          <router-link 
            :to="item.path" 
            class="d-flex align-items-center text-decoration-none rounded p-2 item-link"
            active-class="active"
          >
            <i :class="item.icon" class="fs-5 min-w-icon text-center"></i>
            <span class="links_name ms-3">{{ item.label }}</span>
            <span class="tooltip shadow">{{ item.label }}</span>
          </router-link>
        </li>
      </ul>

      <div class="user-area mt-auto pt-3 border-top border-secondary-subtle">
          
          <router-link 
            to="/app/perfil" 
            class="d-flex align-items-center text-decoration-none rounded p-2 item-link mb-2"
            active-class="active"
          >
            <i class="pi pi-cog fs-5 min-w-icon text-center"></i>
            <span class="links_name ms-3">Configuración</span>
            <span class="tooltip shadow">Configuración</span>
          </router-link>

<div class="profile d-flex align-items-center p-2 rounded bg-profile">
    <div class="profile_details d-flex align-items-center">
        
        <div class="user-avatar rounded d-flex justify-content-center align-items-center bg-white text-dark fw-bold">
            {{ authStore.usuario?.Email?.charAt(0).toUpperCase() || 'U' }}
        </div>

        <div class="name_job ms-3">
            <div class="name fw-bold text-white fs-6 text-truncate" style="max-width: 130px;">
                {{ authStore.usuario?.Email || 'Usuario' }}
            </div>
            
            <div class="job text-light opacity-50 text-xs">
                {{ authStore.usuario?.Rol || 'Undefined' }}
            </div>
        </div>
    </div>
    
    <i class="pi pi-sign-out ms-auto text-white cursor-pointer hover-danger" @click="logout"></i>
</div>

      </div>

    </aside>

    <div class="home-section flex-grow-1 d-flex flex-column h-100vh overflow-hidden">
        <header class="bg-white shadow-sm py-3 px-4 d-flex align-items-center justify-content-between">
            <div class="text text-dark fw-bold fs-5">Panel de Control</div>
        </header>
        <main class="p-4 overflow-auto flex-grow-1 bg-light-gray">
            <router-view></router-view>
        </main>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStores';
import 'primeicons/primeicons.css'

const router = useRouter();
const authStore = useAuthStore();
const isCollapsed = ref(false);

const menuItems = ref([
  { label: 'Inicio', icon: 'pi pi-home', path: '/app/dashboard' },
  { label: 'Pacientes', icon: 'pi pi-users', path: '/app/pacientes' },
  { label: 'Comidas', icon: 'pi pi-apple', path: '/app/comidas' },
  { label: 'Análisis', icon: 'pi pi-chart-pie', path: '/app/analisis' },
  { label: 'Archivos', icon: 'pi pi-folder', path: '/app/archivos' },
]);

const toggleSidebar = () => {
    isCollapsed.value = !isCollapsed.value;
};

const logout = () => {
    authStore.logout();
    router.push('/login');
};
</script>

<style scoped>
:root {
    --sidebar-color: #11101d;
    --primary-color: #695CFE;
    --hover-color: #1d1b31;
    --text-color: #fff;
    --bg-content: #e4e9f7;
}

.font-poppins {
    font-family: 'Poppins', sans-serif;
}

.sidebar {
    width: 250px;
    background: #11101d;
    height: 100vh;
    z-index: 99;
    position: sticky;
    top: 0;
}

.logo_icon {
    min-width: 50px;
    height: 50px;
    line-height: 50px;
    background: linear-gradient(135deg, #695CFE 0%, #a49eff 100%);
    border-radius: 12px;
}
.logo_name { 
    opacity: 1; 
    transition: opacity 0.3s; 
}

.search-box {
    background: #1d1b31;
    height: 50px;
    border-radius: 12px;
    transition: all 0.4s ease;
}
.search-box input { color: #fff; }
.search-box input::placeholder { color: #ccc; }
.search-box:hover { background: #fff; }
.search-box:hover input, .search-box:hover i { color: #11101d; }

.item-link {
    height: 50px;
    color: #fff;
    transition: all 0.4s ease;
    position: relative;
    white-space: nowrap;
    display: flex;
    align-items: center;
}
.item-link:hover {
    background: #fff;
    color: #11101d;
}
.item-link i {
    height: 50px;
    line-height: 50px;
    font-size: 1.2rem;
    min-width: 50px;
    border-radius: 12px;
    text-align: center;
}
.active { background: #fff; color: #11101d; }

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

.bg-profile {
    background: #1d1b31;
    transition: all 0.4s ease;
}
.user-avatar { width: 40px; height: 40px; border-radius: 12px; }
.hover-danger:hover { color: #ff6b6b !important; }

.sidebar.collapsed { width: 78px; }

.sidebar.collapsed .logo_name,
.sidebar.collapsed input,
.sidebar.collapsed .links_name,
.sidebar.collapsed .profile_details {
    opacity: 0;
    pointer-events: none;
    display: none !important;
}

.sidebar.collapsed .search-box { 
    width: 50px; 
    padding: 0; 
    justify-content: center; 
    cursor: pointer;
}
.sidebar.collapsed .search-box i { margin: 0; }

.sidebar.collapsed .logo-details { 
    justify-content: center; 
    padding: 0; 
}

.sidebar.collapsed #btn {
    position: absolute;
    top: 50%;
    right: 0;
    transform: translateY(-50%);
    font-size: 1.5rem;
    text-align: center;
    width: 100%;
    right: unset;
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

.bg-light-gray { background-color: #e4e9f7; }
</style>