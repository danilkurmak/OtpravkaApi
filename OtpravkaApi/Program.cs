var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger можно оставить и в проде (для лабы удобно)
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS редирект только локально (в контейнере чаще мешает)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

// чтобы по корню домена не была "пустота"
app.MapGet("/", () => "OtpravkaApi is running");

app.Run();
