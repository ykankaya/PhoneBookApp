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
using Polly; 
using Polly.Extensions.Http; 
using System; 
using System.Net.Http; 
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

// hata durumunda retry mekanizması için polly
// static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
// {
//
//     return HttpPolicyExtensions
//         .HandleTransientHttpError() 
//         .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) 
//         .WaitAndRetryAsync(
//             retryCount: 3,
//             sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
//             onRetry: (outcome, timespan, retryAttempt, context) => 
//             {
//                 Console.WriteLine($"ContactService çağrısı başarısız oldu ({outcome.Result?.StatusCode}), {timespan.TotalSeconds} saniye sonra {retryAttempt}. deneme yapılacak. Context: {context.OperationKey}");
//             });
// }


// builder.Services.AddHttpClient("ContactServiceClient", client =>
//     {
//     })
//     .AddPolicyHandler(GetRetryPolicy());

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