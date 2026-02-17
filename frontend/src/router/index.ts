import { createRouter, createWebHistory } from "vue-router";
import LoginView from "../views/LoginView.vue";
import RegisterView from "../views/RegisterView.vue";
import MisDocumentosView from "../views/MisDocumentosView.vue";

import AppLayout from "../layouts/AppLayout.vue";
import { useAuthStore } from "../stores/authStores";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/login",
      name: "login",
      component: LoginView,
      meta: { requiresAuth: false },
    },
    {
      path: "/registro",
      name: "registro",
      component: RegisterView,
      meta: { requiresAuth: false },
    },

    {
      path: "/app",
      component: AppLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: "",
          redirect: { name: "dashboard" },
        },
        {
          path: "dashboard",
          name: "dashboard",
          component: () => import("../views/DashboardView.vue"),
        },
        {
          path: "pacientes",
          name: "pacientes",
          component: () => import("../views/PacientesView.vue"),
        },
        {
          path: "comidas",
          name: "comidas",
          component: () => import("../views/admin/AdminComidaView.vue"),
        },
        {
          path: "categorias",
          name: "categorias",
          component: () => import("../views/admin/AdminCategoriaView.vue"),
        },
        {
          path: "dietas",
          name: "dietas",
          component: () => import("../views/DashboardView.vue"),
        },
        {
          path: "configuracion",
          name: "configuracion",
          component: () => import("../views/ConfiguracionView.vue"),
        },
        {
          path: "patologias",
          name: "patologias",
          component: () => import("../views/admin/AdminPatologiasView.vue"),
        },
        {
          path: "objetivos",
          name: "objetivos",
          component: () => import("../views/admin/AdminObjetivosView.vue"),
        },
        {
          path: 'archivos',
          name: 'mis-documentos',
          component: MisDocumentosView,
        },
      ],
    },

    {
      path: "/",
      redirect: "/app/dashboard",
    },

    {
      path: "/:pathMatch(.*)*",
      redirect: "/app/dashboard",
    },
  ],
});

router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore();
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);

  if (requiresAuth) {
    if (!authStore.estaAutenticado) {
      try {
        await authStore.verificarSesion();
      } catch (error) { }
    }

    if (authStore.estaAutenticado) {
      next();
    } else {
      next("/login");
    }
  } else {
    if (to.path === "/login" && authStore.estaAutenticado) {
      next("/app/dashboard");
    } else {
      next();
    }
  }
});

export default router;