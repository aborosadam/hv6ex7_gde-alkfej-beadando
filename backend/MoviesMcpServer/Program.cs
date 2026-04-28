using MoviesMcpServer.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient<MoviesApiClient>();


builder.Services.AddScoped<McpToolsService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapControllers();

app.Run();