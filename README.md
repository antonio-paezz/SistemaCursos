# Sistema de Cursos – ConectaPro

Sistema web desarrollado en ASP.NET Core 8 con Entity Framework Core, diseñado para la gestión integral de cursos en un entorno que simula un escenario de producción real.

## Descripción

El proyecto permite:

Plataforma para gestionar cursos online, permitiendo que instructores publiquen contenido y alumnos se inscriban, compren cursos y dejen calificaciones.

Además incluye:

- Modificación de perfiles propios por parte del usuario
- Visualización de actividad reciente por usuario
- Gestión de usuarios y roles por separado (administración)
- Área administrativa con dashboard de métricas
- Reportes de ventas, usuarios registrados e ingresos
- Gráficos estadísticos por mes utilizando Chart.js
- Registro de actividad mediante sistema de logs
- Simulación de ambiente productivo con configuración separada para producción
- Almacenamiento de archivos e imágenes en la nube con Cloudinary

El sistema está diseñado bajo arquitectura MVC y buenas prácticas de desarrollo, buscando replicar un producto real que podría desplegarse en producción.

## Funcionalidades principales

### Usuarios
- Registro e inicio de sesión
- Perfil con edición de datos personales
- Visualización de actividad reciente
- Roles (Administrador, Instructor, Alumno)

### Cursos
- Creación y gestión de cursos
- Publicación de contenido por instructores
- Inscripción de alumnos
- Carrito de compras
- Calificaciones y reseñas

### Administración
- Gestión de usuarios y roles
- Dashboard con métricas
- Gráficos estadísticos
- Reportes de ventas
- Logs de actividad

### Tecnología y producción
- Configuración diferenciada para desarrollo y producción
- Base de datos SQL Server
- Procedimientos almacenados
- Integración con Cloudinary
- Frontend con Bootstrap y JavaScript

## Tecnologías utilizadas

- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JavaScript
- Bootstrap
- Chart.js
- Cloudinary

## Instalación y ejecución

### 1️⃣ Clonar el repositorio

```bash
git clone URL_DEL_REPO
2️⃣ Configurar base de datos

En appsettings.json, configurar la connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVER;Database=TU_DB;User Id=USUARIO;Password=PASS;TrustServerCertificate=True;"
}

Si se despliega en producción (por ejemplo en hosting), utilizar la cadena correspondiente del entorno.

3️⃣ Aplicar migraciones (si es necesario)
dotnet ef database update
4️⃣ Ejecutar el proyecto
dotnet run

Luego acceder en el navegador:

https://localhost:5001

Repositorio

Proyecto gestionado en GitHub.

Autor

Proyecto desarrollado como trabajo académico, simulando un escenario de producción real para demostrar buenas prácticas en desarrollo web.
