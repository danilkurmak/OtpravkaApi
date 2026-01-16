var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ВАЖНО: сначала статика
app.UseDefaultFiles(); // будет искать index.html в wwwroot
app.UseStaticFiles();  // отдаёт файлы из wwwroot

// Swagger включаем
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
