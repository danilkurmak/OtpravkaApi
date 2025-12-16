var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger включаем всегда (для Amvera это нормально)
app.UseSwagger();
app.UseSwaggerUI();

// авторизация (пусть будет)
app.UseAuthorization();

app.MapControllers();

// редирект с корня на swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
