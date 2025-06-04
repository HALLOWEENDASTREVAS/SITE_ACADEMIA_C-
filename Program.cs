using Microsoft.EntityFrameworkCore;
using AcademiaAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar banco SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.WebHost.UseUrls("http://localhost:5000", "http://localhost:5001", "http://127.0.0.1:5500");

// Adicione aqui os domínios permitidos (obs.: no main.js, o domínio é fixo tambem)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontEnd", policy =>
    {
        policy
          .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa o CORS antes do MAPCONTROLLERS (Não mecher, pois foi o que resolveu o problema de CORS)
app.UseCors("PermitirFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
