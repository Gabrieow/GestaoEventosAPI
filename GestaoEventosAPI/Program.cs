using GestaoEventosAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// connection string SQLite
var connectionString = "Data Source=gestaoeventos.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();