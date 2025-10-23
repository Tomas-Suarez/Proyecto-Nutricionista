# Sistema de Gestión para Nutricionistas

## Descripción del Proyecto

Este es un sistema web (construido con ASP.NET Core y Vue.js) diseñado para la gestión integral de pacientes por parte de nutricionistas.

El objetivo principal es que cada profesional pueda tener su propia cuenta aislada, donde puede registrar a sus pacientes, crear dietas personalizadas y llevar un historial detallado de los pesajes. A su vez, los pacientes pueden acceder con una cuenta (creada por su nutricionista) para visualizar la dieta asignada y ver su evolución de peso.

---

## 🚀 Características Principales

### Como Nutricionista (Rol: `Nutricionista`)

* **Gestión de Cuenta:** Creación de cuenta, inicio y cierre de sesión.
* **Gestión de Pacientes (CRUD):** Registrar nuevos pacientes, ver su lista, editar sus datos y darlos de baja. Cada nutricionista solo puede ver y administrar a *sus propios* pacientes.
* **Creación de Cuentas para Pacientes:** Generar credenciales de acceso (usuario y contraseña) para que un paciente pueda ingresar al sistema.
* **Creación de Dietas:** Diseñar planes de alimentación detallados, asignando comidas a diferentes horarios (ej. Desayuno, Almuerzo, Cena).
* **Seguimiento de Progreso:** Registrar los pesajes periódicos de cada paciente, añadir notas y ver el historial.

### Como Paciente (Rol: `Paciente`)

* **Acceso al Sistema:** Iniciar sesión con las credenciales proporcionadas por su nutricionista.
* **Visualización de Dieta:** Ver el plan de alimentación actual asignado por el profesional.
* **Visualización de Evolución:** Ver un historial o un gráfico de sus pesajes para monitorear su progreso.

---

## 💻 Stack Tecnológico

* **Backend:** ASP.NET Core Web API (.NET 8)
* **Frontend:** Vue.js 3 (con Vite y TypeScript)
* **Base de Datos:** Mysql
* **Autenticación:** JWT con Cookies.
* **ORM:** Entity Framework Core

---

## 📦 Entregables del Proyecto

* **Diagrama entidad relación:**
  <img width="1309" height="661" alt="Nutricion-DER drawio" src="https://github.com/user-attachments/assets/d4799a16-5051-4579-abb9-3a596a208b24" />

* **Base de Datos:** `En proceso...`
