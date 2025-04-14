using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReportService.Infrastructure.Persistence.Context;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("ReportDbConnection");

builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
          sqlOptions.MigrationsAssembly(typeof(ReportDbContext).Assembly.FullName);
        }));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ReportService.Application.AssemblyMarker).Assembly));


builder.Services.AddCarter();


builder.Services.AddHttpClient();

// TODO: Kafka Producer/Consumer Configuration ve Servis Kayýtlarý eklenecek
// builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
// builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>(); // Örnek
// builder.Services.AddHostedService<ReportRequestConsumerWorker>(); // Örnek


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