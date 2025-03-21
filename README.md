# Yetify

Yetify es una aplicación educativa con gamificación para niños. Incluye una mascota virtual y módulos de aprendizaje interactivos.

# Instalación y Configuración

# Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) o superior
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Visual Studio Code o Visual Studio

# Clonar el repositorio
```sh
git clone https://github.com/Kevin7819/Yetify.git
cd Yetify/api
Configurar la base de datos

crear appsettings.Development .json

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}


crear appsettings.json y coloca tus credenciales de SQL Server:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=dbyetify;User Id=USUARIO;Password=CONTRASEÑA;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

Cómo ejecutar la API

Instalar dependencias

dotnet restore

Crear la base de datos

dotnet ef database update

dotnet run