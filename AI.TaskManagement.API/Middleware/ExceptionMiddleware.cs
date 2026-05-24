using AI.TaskManagement.Shared.Exceptions;
using AI.TaskManagement.Shared.Responses;
using Serilog;
using System.Text.Json;

namespace AI.TaskManagement.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse();

        switch (exception)
        {
            case ValidationException ve:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ve.Errors
                };
                break;

            case UnauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = ApiResponse.ErrorResponse("Unauthorized access");
                break;

            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = ApiResponse.ErrorResponse(exception.Message);
                break;

            case ConflictException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = ApiResponse.ErrorResponse(exception.Message);
                break;

            case BusinessException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse.ErrorResponse(exception.Message);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                Log.Error(exception, "An unhandled exception occurred");
                response = ApiResponse.ErrorResponse("An internal server error occurred");
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}
