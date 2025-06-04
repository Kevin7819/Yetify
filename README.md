# Yetify

Yetify is an educational app with gamification for children. It includes a virtual pet and interactive learning modules.

## Installation and Configuration

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) or higher  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- Visual Studio Code or Visual Studio  

### Clone the repository

```sh
git clone https://github.com/Kevin7819/Yetify.git
cd Yetify/api
```

### Configure the database

#### 1️⃣ Create `appsettings.Development.json`

Create a file `appsettings.Development.json` inside `api` with the following content:

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

#### 2️⃣ Create `appsettings.json` and configure SQL Server credentials

Inside `api`, create a file named `appsettings.json` and add your credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=\"TUSERVIDOR\";Database=dbyetify;Integrated Security=True;TrustServerCertificate=True;"
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
    "Audience": "YetifyUsers",
    "ExpireMinutes": 60 
  },
  "AllowedHosts": "*",
  "EmailConfiguration": {
    "From": "kvenegasbermudez@gmail.com",
    "DisplayName": "Yetify Support",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "kvenegasbermudez@gmail.com",
    "Password": "gsas gryy bqng evwi",
    "UseSsl": true,  
    "Timeout": 30000  
  }
}
```

#### 2️⃣ Create carpeta Properties en api y dentro de la carpeta un archivo`launchSettings.json` con esto

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:10059",
      "sslPort": 44312
    }
  },
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://0.0.0.0:5003",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7182;http://localhost:5003",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## How to run the API

### 1️⃣ Install dependencies

```sh
dotnet restore
```

### 2️⃣ Create the database

### To start migrations

```sh
dotnet ef migrations add Init
```

### To update the database

```sh
dotnet ef database update
```

### 3️⃣ Run the API

```sh
dotnet run
```