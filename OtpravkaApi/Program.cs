var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseDefaultFiles();   
app.UseStaticFiles();   

// Swagger включаем всегда (и локально, и на Amvera)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

//корень сайта открывает swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/ui", () => Results.Redirect("/index.html"));

app.Run();
