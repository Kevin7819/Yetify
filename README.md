# Yetify

Yetify es una aplicación educativa con gamificación para niños. Incluye una mascota virtual y módulos de aprendizaje interactivos.

##  Instalación y Configuración

###  Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) o superior  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- Visual Studio Code o Visual Studio  

### Clonar el repositorio

```sh
git clone https://github.com/Kevin7819/Yetify.git
cd Yetify/api
```

### Configurar la base de datos

#### 1️⃣ Crear `appsettings.Development.json`

Crea un archivo `appsettings.Development.json` dentro de `api` con el siguiente contenido:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 2️⃣ Crear `appsettings.json` y configurar credenciales de SQL Server

Dentro de `api`, crea un archivo llamado `appsettings.json` y añade tus credenciales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server="TUSERVIDOR";Database=dbyetify;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Jwt": {
    "Key": "13E13E1B-4222-43E5-8AA5-B8E5328D9E84",
    "Issuer": "YetifyAPI",
    "Audience": "YetifyUsers"
  },
  "AllowedHosts": "*"
}
```

## Cómo ejecutar la API

### 1️⃣ Instalar dependencias

```sh
dotnet restore
```

### 2️⃣ Crear la base de datos

```sh
dotnet ef database update
```

### 3️⃣ Ejecutar la API

```sh
dotnet run
```

