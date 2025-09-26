using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Api_Transferencia.Application.Handlers;
using Api_Transferencia.Domain.Repositories;
using Api_Transferencia.Domain.Services;
using Api_Transferencia.Infrastructure.Data;
using Api_Transferencia.Infrastructure.Repositories;
using Api_Transferencia.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Api Transferencia", Version = "v1" });
    
    // Configuração para JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Configuração do banco de dados Oracle
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=192.168.16.109:1521/desafio;User Id=transferencia;Password=transferencia123456;Pooling=true;";

builder.Services.AddSingleton<IDbConnectionFactory>(provider => 
    new OracleConnectionFactory(connectionString));

// Configuração JWT - Lendo corretamente do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? "MinhaChaveSecretaSuperSegura123456789BancoDigital";
var issuer = jwtSettings.GetValue<string>("Issuer") ?? "Api_ContaCorrente";
var audience = jwtSettings.GetValue<string>("Audience") ?? "Api_ContaCorrente_Users";

Console.WriteLine($"JWT Settings - SecretKey: {secretKey}");
Console.WriteLine($"JWT Settings - Issuer: {issuer}");
Console.WriteLine($"JWT Settings - Audience: {audience}");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configuração MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TransferirHandler).Assembly));

// Injeção de dependência - Repositories
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

// Injeção de dependência - Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient<IContaCorrenteService, ContaCorrenteService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiContaCorrente:BaseUrl"] ?? "http://localhost:5222");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
