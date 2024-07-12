using OperationsApi.Middleware;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseWhen( static context => context.Request.Path.StartsWithSegments( "api/tasks" ),
    _ => { app.UseMiddleware<EmployeeAuthenticationMiddleware>(); } );

app.UseMiddleware<EmployeeAuthenticationMiddleware>();

app.UseHttpsRedirection();
app.Run();