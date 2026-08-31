# Pet Care & Veterinary Management System

An ASP.NET Core MVC application for managing a veterinary/pet-grooming clinic: clients, pets, employees, procedures, and reports. Built with Entity Framework Core and SQL Server.

- Video walkthrough: https://youtu.be/rjhfiBwzL8k

## Features

- **Clientes** — client (pet owner) records
- **Mascotas** — pet records, linked to their owner
- **Empleados** — staff/employee records
- **Procedimientos** — services/procedures performed, with pricing and IVA (tax) calculation
- **Reportes** — billing/report generation across clients, pets, and procedures

## Tech stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8 (SQL Server provider)
- Razor views + Bootstrap

## Running locally

**Prerequisites:** [.NET 8 SDK](https://dotnet.microsoft.com/download), SQL Server (LocalDB, Express, or full).

1. Create the `Pets` database using the schema script in
   [`SCRIPT CREACION DE BASE DE DATOS.Proyecto3.pdf`](./SCRIPT%20CREACION%20DE%20BASE%20DE%20DATOS.Proyecto3.pdf).
2. Update the connection string in
   [`Proyecto1-1-1548-0877/Proyecto1-1-1548-0877/appsettings.json`](./Proyecto1-1-1548-0877/Proyecto1-1-1548-0877/appsettings.json)
   if your SQL Server instance name differs from `localhost\SQLEXPRESS`.
3. Run the app:

   ```bash
   cd "Proyecto1-1-1548-0877/Proyecto1-1-1548-0877"
   dotnet run
   ```

4. Open the URL printed in the console (e.g. `http://localhost:5036`).

## Deploying to Azure (live demo)

This app deploys as-is to **Azure App Service** with **Azure SQL Database** — no code changes needed, only configuration.

1. **Create an Azure SQL Database.** In the Azure Portal, create a SQL Database (the free/serverless tier is enough for a demo). Note the server name, database name, admin username, and password.
2. **Load the schema.** Open the database's **Query editor** in the portal (or connect with Azure Data Studio/SSMS) and run the script in `SCRIPT CREACION DE BASE DE DATOS.Proyecto3.pdf` against it.
3. **Create an Azure App Service** (Linux or Windows, .NET 8 runtime stack, Free F1 tier works for a demo).
4. **Set the connection string.** In the App Service's **Configuration → Application settings**, add a new setting:
   - Name: `ConnectionStrings__DefaultConnection`
   - Value: `Server=tcp:<your-server>.database.windows.net,1433;Database=Pets;User ID=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;`

   ASP.NET Core automatically maps `__` (double underscore) in environment/app-setting names to the nested `ConnectionStrings:DefaultConnection` key, overriding the local value in `appsettings.json`. No code change required.
5. **Connect deployment to GitHub.** In the App Service's **Deployment Center**, choose GitHub as the source and select this repo/branch — Azure will generate a GitHub Actions workflow automatically (or use the one included at `.github/workflows/azure-webapps-deploy.yml`, filling in `AZURE_WEBAPP_NAME` and the `AZURE_WEBAPP_PUBLISH_PROFILE` secret from the App Service's publish profile).
6. Push to `main` — the app builds and deploys automatically, and the site is live at `https://<your-app-name>.azurewebsites.net`.

## Project structure

```
Proyecto1-1-1548-0877/                 # Solution folder
  Proyecto1-1-1548-0877.sln
  Proyecto1-1-1548-0877/               # ASP.NET Core MVC project
    Controllers/
    Models/
    Views/
    Program.cs
    appsettings.json
```
