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
#### Add these line into appsettings.json file
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
    "DBConnection": "Server=BIPLOB\\SQL2019;Database=BookStoreDB;User Id=sa;Password=213332;TrustServerCertificate=True;"
  }
}
```
#### Program.cs
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
#### Migrations:
```
add-migration init
update-database
```
