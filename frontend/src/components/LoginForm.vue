<template>
  <Card class="w-full max-w-lg shadow-xl">
    <template #title>
      <h2 class="text-3xl font-bold text-center mb-6 text-surface-900 dark:text-surface-0">
        Nutri App
      </h2>
    </template>
    <template #content>
      <Form 
        v-slot="$form"
        :resolver="resolver"
        :initialValues="initialValues"
        @submit="onFormSubmit"
      >
        <div class="flex flex-col gap-10">
            
            <div class="flex flex-col gap-2">
                <label for="email" class="font-semibold text-lg ml-1"></label>
                <InputText 
                    name="email" 
                    type="text" 
                    placeholder="usuario@correo.com" 
                    fluid 
                    class="p-3" 
                />
                <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
                    {{ $form.email.error.message }}
                </Message>
            </div>

            <div class="flex flex-col gap-2">
                <label for="password" class="font-semibold text-lg ml-1"></label>
                <Password 
                    name="password" 
                    :feedback="false" 
                    toggleMask 
                    fluid 
                    placeholder="••••••••"
                    inputClass="p-3 w-full" 
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
                class="mt-2 p-3 text-lg font-bold" 
            />
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
    if (e.valid) {
        emit('submit', e.values);
    }
};
</script>