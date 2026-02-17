using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json;
using Ledgerium.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Ledgerium.Web.Middleware;

public class ProblemDetailsExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProblemDetailsExceptionMiddleware> _logger;

    public ProblemDetailsExceptionMiddleware(
        RequestDelegate next,
        ILogger<ProblemDetailsExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var problem = MapToProblemDetails(context, ex);

            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.ProblemJson;

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }

    private static ProblemDetails MapToProblemDetails(HttpContext context, Exception ex)
    {
        var (status, title, type) = ex switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation error", "validation-error"),
            InsufficientFundsException => (StatusCodes.Status409Conflict, "Insufficient funds", "insufficient-funds"),
            NotFoundException => (StatusCodes.Status404NotFound, "Not found", "not-found"),
            DomainException => (StatusCodes.Status400BadRequest, "Domain error", "domain-error"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument", "invalid-argument"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error", "internal-error")
        };

        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = ex.Message,
            Type = $"https://ledgerium.dev/problems/{type}",
            Instance = context.Request.Path
        };
    }
}