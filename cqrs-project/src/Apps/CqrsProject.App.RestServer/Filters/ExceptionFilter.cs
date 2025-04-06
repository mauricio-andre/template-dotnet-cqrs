using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using FluentValidation;
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
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = _stringLocalizer["message:validation:fluentValidationExceptionTitle"].Value,
                Status = StatusCodes.Status400BadRequest,
                Detail = _stringLocalizer["message:validation:fluentValidationExceptionDetail"].Value,
                Instance = context.HttpContext.Request.Path.Value,
                Extensions = new Dictionary<string, object?>()
                {
                    {
                        "errors",
                        validationException.Errors
                            .GroupBy(error => error.PropertyName)
                            .ToDictionary(
                                grp => GetNormalizedKey(grp.Key),
                                grp => grp.Select(x => x.ErrorMessage).ToArray()
                            )
                    }
                }
            });

            context.ExceptionHandled = true;
        }
        else if (context.Exception is DuplicatedEntityException duplicatedEntityException)
        {
            context.Result = new ConflictObjectResult(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                Title = _stringLocalizer["message:validation:businessExceptionTitle"].Value,
                Status = StatusCodes.Status409Conflict,
                Detail = duplicatedEntityException.Message,
                Instance = context.HttpContext.Request.Path.Value,
                Extensions = new Dictionary<string, object?>()
                {
                    {
                        "errors",
                        duplicatedEntityException.Errors
                            .ToDictionary(
                                item => GetNormalizedKey(item.Key),
                                item => item.Value
                            )
                    }
                }
            });

            context.ExceptionHandled = true;
        }
        else if (context.Exception is BusinessException businessException)
        {
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = _stringLocalizer["message:validation:businessExceptionTitle"].Value,
                Status = StatusCodes.Status400BadRequest,
                Detail = businessException.Message,
                Instance = context.HttpContext.Request.Path.Value,
                Extensions = new Dictionary<string, object?>()
                {
                    {
                        "errors",
                        businessException.Errors
                            .ToDictionary(
                                item => GetNormalizedKey(item.Key),
                                item => item.Value
                            )
                    }
                }
            });

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
