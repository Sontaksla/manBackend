using manBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BackendDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.UseCors(builder => 
//{
//    builder.WithOrigins("https://mywebsite.com").AllowCredentials().AllowAnyHeader().AllowAnyMethod();
//});

app.Run();
