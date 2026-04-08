using Microsoft.EntityFrameworkCore;
using BanSach.Common;
using BanSach.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();

builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

app.MapControllers();
app.Run();