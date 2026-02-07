import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';
import AppLayout from '../layouts/AppLayout.vue';
import { useAuthStore } from '../stores/authStores';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      component: AppLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '', 
          redirect: '/dashboard'
        },
        {
          path: 'dashboard',
          name: 'dashboard',
          component: () => import('../views/DashboardView.vue')
        },
        {
          path: 'pacientes',
          name: 'pacientes',
          component: () => import('../views/DashboardView.vue') 
        },
        {
          path: 'dietas',
          name: 'dietas',
          component: () => import('../views/DashboardView.vue')
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/'
    }
  ]
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);

  if (requiresAuth) {
    if (!authStore.estaAutenticado) {
      try {
        await authStore.verificarSesion();
      } catch (error) {
      }
    }

    if (authStore.estaAutenticado) {
      next();
    } else {
      next('/login');
    }
  } 
  else {    
    if (to.path === '/login' && authStore.estaAutenticado) {
      next('/dashboard');
    } else {
      next();
    }
  }
});

export default router;