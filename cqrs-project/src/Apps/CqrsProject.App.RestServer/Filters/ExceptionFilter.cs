using System.Diagnostics;
using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace CqrsProject.App.RestServer.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    public ExceptionFilter(IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                title = _stringLocalizer["message:validation:fluentValidationExceptionTitle"].Value,
                status = StatusCodes.Status400BadRequest,
                detail = _stringLocalizer["message:validation:fluentValidationExceptionDetail"].Value,
                instance = context.HttpContext.Request.Path.Value,
                errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        grp => GetNormalizedKey(grp.Key),
                        grp => grp.Select(x => x.ErrorMessage).ToArray()
                    ),
                traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
            });

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.ExceptionHandled = true;
        }
        else if (context.Exception is BusinessException BusinessException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                title = _stringLocalizer["message:validation:businessExceptionTitle"].Value,
                status = StatusCodes.Status400BadRequest,
                detail = BusinessException.Message,
                instance = context.HttpContext.Request.Path.Value,
                errors = BusinessException.Errors
                    .ToDictionary(
                        item => GetNormalizedKey(item.Key),
                        item => item.Value
                    ),
                traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
            });

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.ExceptionHandled = true;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.ExceptionHandled = true;
        }
    }

    private static string GetNormalizedKey(string key) => string.Concat(
        char.ToLowerInvariant(key[0]),
        key.Substring(1)
    );
}
