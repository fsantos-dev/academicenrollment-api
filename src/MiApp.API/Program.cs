using FluentValidation;
using MiApp.Application.Interfaces;
using MiApp.Application.Validators.Enrollment;
using MiApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using MiApp.Infrastructure.Data;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Repositories;
using MiApp.Application.Services.Subject;
using MiApp.Application.Services.Auth;
using MiApp.Application.Services.Enrollment;
using MiApp.Application.Services.Classmate;
using MiApp.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentRequestValidator>();

// Infrastructure
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IClassmateService, ClassmateService>();

// Current user
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
var secretKey = jwtConfig["Key"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("La configuración Jwt:SecretKey no existe.");
}
var issuer = jwtConfig["Issuer"];
var audience = jwtConfig["Audience"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))   
    };

    options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse(); // evita que el middleware default escriba su propia respuesta
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var problem = new ProblemDetails
                {
                    Title = "No autorizado",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "No se proporcionó un token válido",
                    Instance = context.Request.Path,
                    Type = $"https://httpstatuses.com/{(int)StatusCodes.Status401Unauthorized}"
                };


                await context.Response.WriteAsJsonAsync(problem);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var problem = new ProblemDetails
                {
                    Title = "Prohibido",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = "No tienes permisos para realizar esta acción",
                    Instance = context.Request.Path,
                    Type = $"https://httpstatuses.com/{(int)StatusCodes.Status403Forbidden}"
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseCustomPipeline();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();