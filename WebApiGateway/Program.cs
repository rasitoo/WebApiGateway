using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddOcelot();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


// Usar CORS

builder.Services.AddMvcCore().AddApiExplorer();
var app = builder.Build();
app.UseCors("AllowLocalhost");
app.UseWebSockets();
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();
await app.RunAsync();
