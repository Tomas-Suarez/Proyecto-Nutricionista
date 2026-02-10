<template>
  <Card class="shadow border-0" style="width: 100%; max-width: 500px;">
    <template #title>
      <h2 class="text-center fw-bold mb-2">Crear Cuenta</h2>
      <p class="text-center text-muted fs-6 mb-4">Registro para Profesionales</p>
    </template>
    
    <template #content>
      <Form 
        v-slot="$form"
        :resolver="resolver"
        :initialValues="initialValues"
        @submit="onFormSubmit"
        class="d-flex flex-column gap-3"
      >
        <div class="row">
            <div class="col-6">
                <label for="nombre" class="form-label fw-bold ms-1">Nombre</label>
                <InputText name="nombre" type="text" placeholder="Juan" fluid />
                <Message v-if="$form.nombre?.invalid" severity="error" size="small" variant="simple">
                    {{ $form.nombre.error.message }}
                </Message>
            </div>
            <div class="col-6">
                <label for="apellido" class="form-label fw-bold ms-1">Apellido</label>
                <InputText name="apellido" type="text" placeholder="Pérez" fluid />
                <Message v-if="$form.apellido?.invalid" severity="error" size="small" variant="simple">
                    {{ $form.apellido.error.message }}
                </Message>
            </div>
        </div>

        <div>
            <label for="email" class="form-label fw-bold ms-1">Email</label>
            <InputText name="email" type="text" placeholder="juan@nutricion.com" fluid />
            <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
                {{ $form.email.error.message }}
            </Message>
        </div>

        <div class="row">
            <div class="col-6">
                <label for="matricula" class="form-label fw-bold ms-1">Matrícula</label>
                <InputText name="matricula" type="text" placeholder="MN-1234" fluid />
                <Message v-if="$form.matricula?.invalid" severity="error" size="small" variant="simple">
                    {{ $form.matricula.error.message }}
                </Message>
            </div>
            <div class="col-6">
                <label for="telefono" class="form-label fw-bold ms-1">Teléfono</label>
                <InputText name="telefono" type="text" placeholder="11-1234-5678" fluid />
                <Message v-if="$form.telefono?.invalid" severity="error" size="small" variant="simple">
                    {{ $form.telefono.error.message }}
                </Message>
            </div>
        </div>

        <div>
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
            label="Registrarse" 
            icon="pi pi-user-plus" 
            :loading="loading" 
            severity="primary" 
            class="w-100 fw-bold py-2 mt-2"
        />

        <div class="text-center mt-3">
            <span class="text-muted">¿Ya tienes cuenta? </span>
            <router-link to="/login" class="text-decoration-none fw-bold">Inicia Sesión</router-link>
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

const initialValues = ref({
    nombre: '',
    apellido: '',
    email: '',
    matricula: '',
    telefono: '',
    password: ''
});

const resolver = zodResolver(
    z.object({
        nombre: z.string().min(2, 'Mínimo 2 caracteres'),
        apellido: z.string().min(2, 'Mínimo 2 caracteres'),
        email: z.string().min(1, 'Requerido').email('Correo inválido'),
        matricula: z.string().min(3, 'Requerido'),
        telefono: z.string().min(6, 'Requerido'),
        password: z.string().min(6, 'Mínimo 6 caracteres')
    })
);

const onFormSubmit = (e: FormSubmitEvent) => {
    if (e.valid) {
        emit('submit', {
            Nombre: e.values.nombre,
            Apellido: e.values.apellido,
            Email: e.values.email,
            Matricula: e.values.matricula,
            Telefono: e.values.telefono,
            Password: e.values.password
        });
    }
};
</script>