var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 1) статика (wwwroot)
app.UseDefaultFiles();   // чтобы "/" открывал index.html
app.UseStaticFiles();    // чтобы вообще отдавались файлы из wwwroot

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

// если вдруг / не отдался статикой — пусть хотя бы ведёт на интерфейс
app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();
