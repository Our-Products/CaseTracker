using CaseTracker.Extensions;
using CaseTrackerInfrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ***** SERVICE REGISTRATION *****
// Infrastructure: DbContext and repositories
builder.Services.AddInfrastructureServices(builder.Configuration);

// Application: Services
builder.Services.AddApplicationServices();

// Authentication: JWT Bearer tokens
builder.Services.AddJwtAuthentication(builder.Configuration);

// Authorization: Policies
builder.Services.AddCustomAuthorization();

// ***** API & DOCUMENTATION *****
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ***** BUILD APP & CONFIGURE PIPELINE *****
var app = builder.Build();

// Exception handling middleware (must be first)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize virtual database schema and seed roles
await CaseTrackerInfrastructure.Data.DbInitializer.InitializeDatabaseAsync(app.Services);

app.Run();
