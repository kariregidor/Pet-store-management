using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Proyecto2_1_1548_0877.Models;
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);


// SERVICE CONFIGURATION


// Controllers (API)
builder.Services.AddControllers();

// Endpoint explorer and Swagger (to test the API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient (to consume other APIs if needed)
builder.Services.AddHttpClient();

// Database context (Entity Framework Core)
builder.Services.AddDbContext<PetsContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// BUILD THE APP


var app = builder.Build();


// REQUEST PIPELINE CONFIGURATION

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


//REFERENCIAS
//JFLHV. (2023).C# - New SQLite database Entity Framework Core [Video]. YouTube. https://www.youtube.com/watch?v=AYBgrQr14BE
//Round The Code. (2023). Create a DbContext class in Entity Framework Core [Video]. YouTube. https://www.youtube.com/watch?v=I-Epp8vcSe0&t=253s
//Garc�a, F. (2019). Como crear base de datos en SQL Server desde cero [Video]. YouTube. https://www.youtube.com/watch?v=fyvEhDgKl7E
//Mendez, D. (2025.). Tercera sesion virtual. Campos Virtual. UNED.[Video]. https://aprende.uned.ac.cr/course/view.php?id=6614&section=4#tabs-tree-start
