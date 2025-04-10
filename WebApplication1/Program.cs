using Microsoft.OpenApi.Models;
using PruebaTec.Data.Interafaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodos",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Prueba Unilink",
        Version = "v1",
        Description = "API para prueba tecnica unilink"
    });

    var archivoXml = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);
    c.IncludeXmlComments(rutaXml);
});

// Agregar el servicio de logging para la aplicación
builder.Logging.AddConsole();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.UseCors("PermitirTodos");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "API Prueba Unilink v1");
    c.RoutePrefix = "swagger"; // Hacer que Swagger UI esté disponible en la raíz
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
