using SlidersControl.ApiEndpoints;
using SlidersControl.AppServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAutenticationJwt();

var app = builder.Build();

app.MapSlidersEndpoints();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
var environment = app.Environment;
app.UseExceptionHandling(environment)
    .UseSwaggerMiddleware()
    .UseAppCors();

app.UseHttpsRedirection();

app.Run();

