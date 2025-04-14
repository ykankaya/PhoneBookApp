using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReportService.Infrastructure.Persistence.Context;
using ReportService.Application.Interfaces.Infrastructure;
using ReportService.Infrastructure.Messaging;
using ReportService.Application.Features.Reports.Commands.ProcessReport;
using ReportService.Api.Workers;
using ReportService.Application.Common.Behaviours;
using ReportService.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("ReportDbConnection");

builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
          sqlOptions.MigrationsAssembly(typeof(ReportDbContext).Assembly.FullName);
        }));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ReportService.Application.AssemblyMarker).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(ReportService.Application.AssemblyMarker).Assembly);


builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

builder.Services.AddCarter();


builder.Services.AddHttpClient();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.Configure<ContactServiceOptions>(builder.Configuration.GetSection("ContactService"));

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddHostedService<ReportRequestConsumerWorker>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ReportService.Api.Middleware.ExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapCarter();

app.Run();