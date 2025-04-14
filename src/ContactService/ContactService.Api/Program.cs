using Carter;
using Microsoft.EntityFrameworkCore;
using ContactService.Infrastructure.Persistence.Context;
using FluentValidation; 
using MediatR; 
using ContactService.Application.Common.Behaviours; 
var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("ContactDbConnection");


builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseNpgsql(connectionString,
    
        npgsqlOptionsAction: sqlOptions =>
        {
          sqlOptions.MigrationsAssembly(typeof(ContactDbContext).Assembly.FullName);
        }));


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ContactService.Application.AssemblyMarker).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(ContactService.Application.AssemblyMarker).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddCarter();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseMiddleware<ContactService.Api.Middleware.ExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapCarter();

app.Run();