using CaseTracker.Extensions;
using CaseTracker.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// SERVICE REGISTRATION
// ==============================

// Infrastructure
 builder.Services.AddInfrastructureServices(
    builder.Configuration);

// Application
builder.Services.AddApplicationServices();

// Authentication
builder.Services.AddJwtAuthentication(
    builder.Configuration);

// Authorization
builder.Services.AddCustomAuthorization();

// ==============================
// API & DOCUMENTATION
// ==============================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// ==============================
// BUILD APPLICATION
// ==============================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Expo", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Expo");

// ==============================
// HTTP REQUEST PIPELINE
// ==============================

// Global exception handling
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();