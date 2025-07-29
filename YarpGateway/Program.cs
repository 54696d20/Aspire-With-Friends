var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add CORS for Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5071", "http://localhost:80")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

// Add reverse proxy with enhanced configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Use CORS
app.UseCors("AllowBlazorApp");

// Add request logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);
    
    try
    {
        await next();
        
        logger.LogInformation("Response: {StatusCode} for {Method} {Path}", 
            context.Response.StatusCode, context.Request.Method, context.Request.Path);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
        throw;
    }
});

app.MapGet("/", () => "YARP Gateway - API Gateway for Aspire With Friends");

// Add health check endpoint
app.MapHealthChecks("/health");

app.MapReverseProxy();

app.Run();