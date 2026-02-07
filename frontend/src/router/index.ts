import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/authStores';
import LoginView from '../views/LoginView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('../views/DashboardView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/',
      redirect: '/login'
    }
  ]
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();

  if (!authStore.usuario && !authStore.cargando) {
    await authStore.verificarSesion();
  }

  if (to.meta.requiresAuth && !authStore.estaAutenticado) {
    next('/login');
  } else {
    next();
  }
});

export default router;