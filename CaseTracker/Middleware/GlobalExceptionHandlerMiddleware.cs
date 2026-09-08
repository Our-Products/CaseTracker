using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CaseTracker.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. TraceId: {TraceId}",
                    context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An unexpected error occurred.";

            Dictionary<string, string[]>? errors = null;

            switch (exception)
            {
                // ----------------------------------------
                // 400 - Validation
                // ----------------------------------------

                case ValidationException validationException:

                    statusCode =
                        StatusCodes.Status400BadRequest;

                    message =
                        validationException.Message;

                    errors =
                        validationException.Errors;

                    break;

                // ----------------------------------------
                // 401 - Unauthorized
                // ----------------------------------------

                case UnauthorizedException unauthorizedException:

                    statusCode =
                        StatusCodes.Status401Unauthorized;

                    message =
                        unauthorizedException.Message;

                    break;

                // ----------------------------------------
                // 403 - Forbidden
                // ----------------------------------------

                case ForbiddenException forbiddenException:

                    statusCode =
                        StatusCodes.Status403Forbidden;

                    message =
                        forbiddenException.Message;

                    break;

                // ----------------------------------------
                // 404 - Not Found
                // ----------------------------------------

                case NotFoundException notFoundException:

                    statusCode =
                        StatusCodes.Status404NotFound;

                    message =
                        notFoundException.Message;

                    break;

                // ----------------------------------------
                // 409 - Conflict
                // ----------------------------------------

                case ConflictException conflictException:

                    statusCode =
                        StatusCodes.Status409Conflict;

                    message =
                        conflictException.Message;

                    break;

                // ----------------------------------------
                // 400 - Invalid Argument
                // ----------------------------------------

                case ArgumentException argumentException:

                    statusCode =
                        StatusCodes.Status400BadRequest;

                    message =
                        "Invalid request.";

                    errors = new Dictionary<string, string[]>
                    {
                        {
                            "request",
                            new[] { argumentException.Message }
                        }
                    };

                    break;

                // ----------------------------------------
                // 409 - Database Conflict
                // ----------------------------------------

                case DbUpdateException:

                    statusCode =
                        StatusCodes.Status409Conflict;

                    message =
                        "The request could not be completed because it conflicts with existing data.";

                    break;

                // ----------------------------------------
                // 500 - Unexpected Error
                // ----------------------------------------

                default:

                    statusCode =
                        StatusCodes.Status500InternalServerError;

                    message =
                        "An unexpected error occurred.";

                    break;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response =
                ApiResponse<object>.FailureResponse(
                    message,
                    errors);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            };

            var json =
                JsonSerializer.Serialize(
                    response,
                    options);

            await context.Response.WriteAsync(json);
        }
    }
}