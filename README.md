# BookStore
#### When we create REST APIs, then we don’t want that any one can access those apis. REST APIs, will only be accessed by the authenticated user. We authenticate our user with the help of jwt.
## How jwt works?

First, we give an authentication endpoint to user, where he/she puts credential, in return we give a jwt token to user which have an expiry date. To consume any protected resource, user need to pass jwt token on authorization header.

#### Book-Store-Spa-Backend is our root folder, inside it we have a sln file and 2 different projects.

(i) BookStore.Api (UI layer)

(ii) BookStore.Data (Data layer)

#### Add the following nuget packages in BookStore.Data
```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore
```
#### Add the following packages in BookStore.Api project.
```
Microsoft.EntityFrameworkCore.Design

Microsoft.AspNetCore.Authentication.JwtBearer
```
## Add these line into appsettings.json file
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "ValidIssuer": "https://localhost:5102",
    "ValidAudience": "https://localhost:5102",
    "ExpireHour": "5",
    "Secret": "ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM"
  },
  "ConnectionStrings": {
    //"DBConnection": "Server=BIPLOB\\SQL2019;Database=BookStoreDB2;User Id=sa;Password=data;TrustServerCertificate=True;"
    "DBConnection": "Server=tcp:bookstore-db-server.database.windows.net,1433;Initial Catalog=BookStoreDB;Persist Security Info=False;User ID=biplob;Password=BookStore@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```
## Program.cs
```
using BookStore.Api.Services;
using BookStore.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// For DBConnection 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
// Adding Authentication  
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer  
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

// Add services to the container.
builder.Services.AddTransient<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```
## Migrations:
```
add-migration init
update-database
```
## ID/CD with GitHub Action
![image](https://github.com/user-attachments/assets/03381838-f890-42f5-9ad8-16843d8434ae)

## master_bookstoreapp.yml
```
name: Build and deploy ASP.Net Core app to Azure Web App - BookStoreApp

on:
  push:
    branches:
      - master
  workflow_dispatch:

jobs:
  build:
    runs-on: windows-latest
    permissions:
      contents: read

    steps:
      - name: Checkout source
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release

      - name: Publish
        run: dotnet publish ./BookStore.Api/BookStore.Api.csproj -c Release -o ./publish

      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: net-app
          path: ./publish

  deploy:
    runs-on: windows-latest
    needs: build
    environment:
      name: 'Production'
      url: ${{ steps.deploy-to-webapp.outputs.webapp-url }}
    permissions:
      id-token: write
      contents: read

    steps:
      - name: Checkout source
        uses: actions/checkout@v4

      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: net-app
          path: ./publish

      - name: Login to Azure
        uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZUREAPPSERVICE_CLIENTID_03DA2E868BBA4180AB1D8EEE5D1E5C90 }}
          tenant-id: ${{ secrets.AZUREAPPSERVICE_TENANTID_E665BD84DEB846C495C3B722332D9FA4 }}
          subscription-id: ${{ secrets.AZUREAPPSERVICE_SUBSCRIPTIONID_C718C757446A4CE592C89CE2F15B23E6 }}

      - name: Install dotnet-ef
        run: dotnet tool install --global dotnet-ef --version 8.0.0

      - name: Add dotnet tools to PATH
        run: echo "$env:USERPROFILE\.dotnet\tools" | Out-File -Append -Encoding ascii $env:GITHUB_PATH

      - name: Apply EF Core Migrations
        run: dotnet ef database update --project "BookStore.Data/BookStore.Data.csproj" --startup-project "BookStore.Api/BookStore.Api.csproj" --connection "$env:DB_CONNECTION_STRING"
        env:
          DB_CONNECTION_STRING: ${{ secrets.AZURE_SQL_CONNECTION_STRING }}

      - name: Deploy to Azure Web App
        id: deploy-to-webapp
        uses: azure/webapps-deploy@v3
        with:
          app-name: 'BookStoreApp'
          slot-name: 'Production'
          package: ./publish
```
## Azure App Service
![image](https://github.com/user-attachments/assets/47d6aaa3-c995-4a8e-bf80-4f3398561e70)

![image](https://github.com/user-attachments/assets/c7b53823-1629-4d9c-8e82-d6be114b8026)

## Azure SQL Database
![image](https://github.com/user-attachments/assets/86f0fd6f-a2af-40e5-a64c-6e6157a33853)

