using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();
builder.Services.AddCors(options =>
{
  options.AddPolicy("CorsPolicy", policy =>
  {
    policy.AllowAnyOrigin() // Veya belirli origin'leri belirtin
          .AllowAnyMethod()
          .AllowAnyHeader();
  });
});
var app = builder.Build();

app.UseCors("CorsPolicy");
await app.UseOcelot();

app.Run();
