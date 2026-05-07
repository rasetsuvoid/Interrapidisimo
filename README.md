# Interrapidisimo — Portal Académico

Sistema de registro de estudiantes e inscripción de materias construido con **.NET 10** (backend) y **Angular 21** (frontend), siguiendo arquitectura limpia (Clean Architecture).

---

## Tabla de contenidos

1. [Stack tecnológico](#stack-tecnológico)
2. [Arquitectura](#arquitectura)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Prerrequisitos](#prerrequisitos)
5. [Configuración local (sin Docker)](#configuración-local-sin-docker)
6. [Configuración con Docker](#configuración-con-docker)
7. [Variables de entorno](#variables-de-entorno)
8. [Endpoints de la API](#endpoints-de-la-api)
9. [Pruebas unitarias](#pruebas-unitarias)
10. [CI/CD — GitHub Actions](#cicd--github-actions)

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Backend | .NET 10, ASP.NET Core, Entity Framework Core |
| Base de datos | SQL Server 2022 |
| Frontend | Angular 21, Signals, SCSS |
| Autenticación | JWT Bearer |
| Validación | FluentValidation |
| Rate limiting | ASP.NET Core Rate Limiting |
| Pruebas | MSTest, FluentAssertions, Moq |
| Contenedores | Docker, Docker Compose |
| CI/CD | GitHub Actions, GHCR |

---

## Arquitectura

El backend sigue **Clean Architecture** con cuatro capas:

```
Domain          → Entidades, interfaces de repositorios, excepciones de negocio
Application     → Servicios de aplicación, DTOs, validadores, interfaces de servicios
Infrastructure  → EF Core, repositorios, JWT, hashing de contraseñas, seeder
API             → Controladores REST, middleware, program.cs
```

El frontend sigue la estructura de **Feature Modules** de Angular:

```
core/           → Interceptores HTTP, guardias de autenticación, servicios globales
features/       → auth, dashboard, students, subjects (cada uno con su componente y SCSS)
shared/         → Componentes reutilizables (modales, UI)
```

### Patrón repositorio genérico

Todos los repositorios heredan de `Repository<T>` que implementa los métodos comunes (`GetByIdAsync`, `AddAsync`, `Update`, `ExistsAsync`). Los repositorios específicos solo añaden los métodos propios de cada entidad y sobreescriben `GetByIdAsync` cuando necesitan `Include` personalizados.

```
IRepository<T>  ←  IProfessorRepository
                ←  IStudentRepository
                ←  ISubjectRepository
                ←  IEnrollmentRepository
                ←  IStudentAccountRepository

Repository<T>   ←  ProfessorRepository
                ←  StudentRepository
                ←  SubjectRepository
                ←  EnrollmentRepository
                ←  StudentAccountRepository
```

### Patrón Result

Los servicios de aplicación retornan `Result<T>` en lugar de lanzar excepciones, permitiendo al controlador decidir el código HTTP apropiado. Las excepciones de dominio (`BusinessRuleException`, `NotFoundException`, `ConflictException`) solo se usan dentro de las entidades y son capturadas por el `ExceptionHandlingMiddleware`.

---

## Estructura del proyecto

```
Interrapidisimo/
├── src/
│   ├── Domain/
│   │   ├── Common/          # BaseEntity, AuditableEntity
│   │   ├── Entities/        # Student, Subject, Professor, StudentSubject, StudentAccount
│   │   ├── Exceptions/      # BusinessRuleException, NotFoundException, ConflictException
│   │   └── Interfaces/      # IRepository<T>, IUnitOfWork y repositorios específicos
│   ├── Application/
│   │   ├── Auth/            # AuthService, DTOs, validadores
│   │   ├── Students/        # StudentService, DTOs, validadores
│   │   ├── Subjects/        # SubjectService, DTOs
│   │   ├── Professors/      # ProfessorService, DTOs
│   │   ├── Enrollments/     # EnrollmentService, DTOs, validadores
│   │   └── Common/          # Result<T>, ApiResponse, PagedResult, interfaces de servicios
│   ├── Infrastructure/
│   │   ├── Persistence/     # ApplicationDbContext, repositorios, migraciones, seeder
│   │   └── Services/        # JwtTokenService, PasswordHasher
│   └── API/
│       ├── Controllers/     # AuthController, StudentsController, SubjectsController, ProfessorsController, EnrollmentsController
│       ├── Middleware/      # ExceptionHandlingMiddleware
│       ├── Dockerfile
│       └── Program.cs
├── tests/
│   └── Application.Tests/   # Pruebas unitarias de servicios y dominio
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/        # interceptores, guardias, servicios (auth, rate-limit)
│   │   │   ├── features/    # auth, dashboard, students, subjects
│   │   │   └── shared/      # RateLimitModalComponent, componentes UI
│   │   └── styles.css       # Variables CSS globales (paleta naranja)
│   ├── Dockerfile
│   └── nginx.conf
├── docker-compose.yml
├── .env.example
└── .gitignore
```

---

## Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/)
- [SQL Server 2022](https://www.microsoft.com/sql-server) o Docker
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (opcional)

---

## Configuración local (sin Docker)

### 1. Clonar el repositorio

```bash
git clone https://github.com/rasetsuvoid/interrapidisimo.git
cd interrapidisimo
```

### 2. Configurar la cadena de conexión

Crea `src/API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InterrapidisimoDB;User Id=sa;Password=TuPassword123!;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "una-clave-secreta-de-al-menos-32-caracteres-aqui"
  }
}
```

### 3. Aplicar migraciones

Las migraciones se aplican automáticamente al iniciar la API. Para aplicarlas manualmente:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/API
```

### 4. Levantar el backend

```bash
dotnet run --project src/API
```

API disponible en `https://localhost:7001` — Swagger en `https://localhost:7001/swagger`.

### 5. Levantar el frontend

```bash
cd frontend
npm install
ng serve
```

Frontend disponible en `http://localhost:4200`.

---

## Configuración con Docker

### 1. Crear el archivo `.env`

```bash
cp .env.example .env
```

Edita `.env`:

```env
SA_PASSWORD=TuPassword123!
JWT_SECRET=una-clave-secreta-de-al-menos-32-caracteres-aqui
```

> La contraseña de SQL Server debe tener mínimo 8 caracteres con mayúsculas, minúsculas, números y símbolos.

### 2. Levantar los servicios

```bash
docker compose up --build
```

| Servicio | Puerto | Descripción |
|---|---|---|
| `db` | 1433 | SQL Server 2022 |
| `api` | 8080 | ASP.NET Core API |
| `frontend` | 80 | Angular servido por nginx |

- Frontend: `http://localhost`
- API (directa): `http://localhost:8080`
- API (vía nginx): `http://localhost/api`
- Swagger: `http://localhost/swagger`

### 3. Primera ejecución

Al iniciar, el seeder crea automáticamente 5 profesores y 10 materias.

### 4. Detener los servicios

```bash
docker compose down       # Detiene y elimina contenedores
docker compose down -v    # También elimina el volumen de la base de datos
```

---

## Variables de entorno

| Variable | Descripción | Ejemplo |
|---|---|---|
| `SA_PASSWORD` | Contraseña del usuario `sa` de SQL Server | `TuPassword123!` |
| `JWT_SECRET` | Clave para firmar tokens JWT (mínimo 32 caracteres) | `super-secret-key-32-chars-minimum` |

**Nunca commits el archivo `.env` al repositorio.** Está excluido por `.gitignore`.

---

## Endpoints de la API

### Autenticación (público)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/auth/register` | Registro de nuevo estudiante |
| `POST` | `/api/auth/login` | Inicio de sesión — retorna JWT |

### Estudiantes (requiere JWT)

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/students` | Lista paginada (`?page=1&pageSize=10&search=`) |
| `GET` | `/api/students/{id}` | Detalle de un estudiante |
| `POST` | `/api/students` | Crear estudiante |
| `PUT` | `/api/students/{id}` | Actualizar estudiante |
| `DELETE` | `/api/students/{id}` | Eliminar estudiante (soft delete) |

### Materias (requiere JWT)

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/subjects` | Todas las materias con detalles |
| `GET` | `/api/subjects/{id}` | Detalle de una materia |

### Profesores (requiere JWT)

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/professors` | Todos los profesores con sus materias |
| `GET` | `/api/professors/{id}` | Detalle de un profesor |

### Inscripciones (requiere JWT)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/enrollments` | Inscribir estudiante en materia |
| `DELETE` | `/api/enrollments` | Retirar estudiante de materia |
| `GET` | `/api/enrollments/status/{studentId}` | Estado de inscripción del estudiante |
| `GET` | `/api/enrollments/classmates/{studentId}` | Materias con compañeros de clase |

### Reglas de negocio de inscripción

- Máximo **3 materias** activas por estudiante.
- No puede inscribirse en dos materias del **mismo profesor**.
- No puede inscribirse en una materia en la que **ya esté inscrito**.

### Rate limiting

La API aplica **10 solicitudes por minuto** por IP. Al superarlo retorna `429 Too Many Requests` con el encabezado `Retry-After`. El frontend muestra un modal informando al usuario cuántos segundos debe esperar.

---

## Pruebas unitarias

```bash
dotnet test
```

Las pruebas cubren:
- Reglas de dominio de `Student` (máximo materias, mismo profesor, ya inscrito)
- `EnrollmentService` (flujos de éxito y de error)

```
Correctas! - Con error: 0, Superado: 26, Omitido: 0, Total: 26
```

Para ver cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## CI/CD — GitHub Actions

El pipeline (`.github/workflows/frontend.yml`) se ejecuta en cada push o PR sobre `main` que modifique archivos dentro de `frontend/`.

### Job `build`

1. Checkout del código
2. Setup de Node.js 22
3. `npm ci` — instalación de dependencias
4. `ng build --configuration production` — compilación
5. Subida del artefacto compilado (retención 1 día)

### Job `publish` (solo push a `main`)

1. Convierte el nombre del repositorio a minúsculas
2. Login al GitHub Container Registry (GHCR) con `GITHUB_TOKEN`
3. Generación de tags `sha-<commit>` y `latest` con `docker/metadata-action`
4. Build y push de la imagen Docker con caché de capas de GHA

Imagen disponible en:

```
ghcr.io/rasetsuvoid/interrapidisimo:latest
```

---

## Decisiones de diseño

| Decisión | Justificación |
|---|---|
| **Soft delete global** | `IsDeleted` en `BaseEntity` + filtros globales de EF Core. Ninguna consulta retorna registros eliminados sin código extra. |
| **Result\<T\> sin excepciones en Application** | Los servicios retornan `Result<T>` permitiendo al controlador elegir el código HTTP. Las excepciones quedan reservadas al dominio. |
| **`GetByIdAsync` virtual** | Permite que `ProfessorRepository` y `EnrollmentRepository` sobreescriban con sus `Include` sin romper el contrato base. |
| **nginx como reverse proxy** | En Docker, el browser habla solo con `http://localhost`. nginx enruta `/api/*` al contenedor de la API eliminando problemas de CORS en producción. |
| **JWT sin refresh token** | Simplicidad para el alcance del proyecto. En producción se recomienda añadir refresh tokens con rotación. |
| **Rate limiting por IP** | Protección básica contra abuso. Se configura en `Program.cs` con la política `fixed` de ASP.NET Core. |
