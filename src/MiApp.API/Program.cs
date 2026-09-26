using FluentValidation;
using MiApp.Application.Validators.Enrollment;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();


builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentRequestValidator>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();
app.Run();