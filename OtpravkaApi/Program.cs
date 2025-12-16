var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger включаем всегда 
app.UseSwagger();
app.UseSwaggerUI();

// авторизация 
app.UseAuthorization();

app.MapControllers();

// редирект с корня на swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
