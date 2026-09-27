using FluentValidation;
using MiApp.Application.Interfaces;
using MiApp.Application.Validators.Enrollment;
using MiApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using MiApp.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();


builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentRequestValidator>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();
app.Run();