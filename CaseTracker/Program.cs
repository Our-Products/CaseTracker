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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// ==============================
// BUILD APPLICATION
// ==============================

var app = builder.Build();

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