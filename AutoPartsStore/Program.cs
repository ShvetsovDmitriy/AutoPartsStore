using AutoPartsStore.Services.Auth;
using AutoPartsStore.Services.Customer;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString"));
});
builder.Services.AddScoped<ICustomerInfoService, CustomerInfoService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection(nameof(AuthSettings)));


builder.Services.AddControllers();

// Добавление Swagger для документирования API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });

});

// Добавление кэширования данных
builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty; // Убрать префикс маршрута
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


var _serilogLogger = new LoggerConfiguration()
    .WriteTo.File(
        "logs/log.log",
        rollingInterval: RollingInterval.Hour)
    .CreateLogger();

builder.Services.AddLogging(builder => {
    builder.AddSerilog(_serilogLogger);
});