using MoviesApi.Models;
using MoviesApi.Services;

var builder = WebApplication.CreateBuilder(args);

// MongoDB settings betöltése appsettings.json-ból
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Saját service-ek regisztrálása
builder.Services.AddSingleton<MoviesService>();
builder.Services.AddSingleton<ReviewsService>();

// Controller támogatás
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - frontend más portról jön majd
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger UI minden környezetben (kényelmes teszteléshez)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

// Health endpoint - K8s readiness probe-hoz hasznos
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapControllers();

app.Run();