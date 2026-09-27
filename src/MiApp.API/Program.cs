using FluentValidation;
using MiApp.Application.Interfaces;
using MiApp.Application.Validators.Enrollment;
using MiApp.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();


builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentRequestValidator>();
builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();
app.Run();