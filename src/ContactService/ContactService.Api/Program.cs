using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ContactService.Infrastructure.Persistence.Context;
using ContactService.Application; 

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

builder.Services.AddCarter();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapCarter();

app.Run();