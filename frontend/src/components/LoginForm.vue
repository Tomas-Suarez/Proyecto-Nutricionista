<template>
  <Card class="shadow border-0" style="width: 100%; max-width: 450px;">
    <template #title>
      <h2 class="text-center fw-bold mb-4">Nutri App</h2>
    </template>
    
    <template #content>
      <Form 
        v-slot="$form"
        :resolver="resolver"
        :initialValues="initialValues"
        @submit="onFormSubmit"
      >
        <div class="mb-3">
            <label for="email" class="form-label fw-bold ms-1">Email</label>
            <InputText 
                name="email" 
                type="text" 
                placeholder="usuario@correo.com" 
                fluid 
            />
            <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
                {{ $form.email.error.message }}
            </Message>
        </div>

        <div class="mb-4">
            <label for="password" class="form-label fw-bold ms-1">Contraseña</label>
            <Password 
                name="password" 
                :feedback="false" 
                toggleMask 
                fluid 
                placeholder="••••••••"
            />
            <Message v-if="$form.password?.invalid" severity="error" size="small" variant="simple">
                {{ $form.password.error.message }}
            </Message>
        </div>

        <Button 
            type="submit" 
            label="Iniciar Sesión" 
            icon="pi pi-sign-in" 
            :loading="loading" 
            severity="secondary" 
            class="w-100 fw-bold py-2"
        />

        <div class="text-center mt-3">
            <span class="text-muted">¿No tienes cuenta? </span>
            <router-link to="/registro" class="text-decoration-none fw-bold" style="color: var(--p-primary-color);">
                Regístrate aquí
            </router-link>
        </div>
        
      </Form>
    </template>
  </Card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { z } from 'zod';
import { zodResolver } from '@primevue/forms/resolvers/zod';
import { Form, type FormSubmitEvent } from '@primevue/forms';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import Password from 'primevue/password';
import Button from 'primevue/button';
import Message from 'primevue/message';

defineProps<{ loading: boolean }>();
const emit = defineEmits(['submit']);

const initialValues = ref({ email: '', password: '' });

const resolver = zodResolver(
    z.object({
        email: z.string().min(1, 'Requerido').email('Correo inválido'),
        password: z.string().min(1, 'Requerido').min(6, 'Mínimo 6 caracteres')
    })
);

const onFormSubmit = (e: FormSubmitEvent) => {
    if (e.valid) emit('submit', e.values);
};
</script>