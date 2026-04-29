using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace App.Api.Filters
{
    public class ResultLoggingFilter : IAsyncResultFilter
    {
        private readonly ILogger<ResultLoggingFilter> _logger;

        public ResultLoggingFilter(ILogger<ResultLoggingFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var responseType = objectResult.Value.GetType();
                var successProp = responseType.GetProperty("Success");
                
                if (successProp != null)
                {
                    bool isSuccess = (bool)(successProp.GetValue(objectResult.Value) ?? true);
                    
                    if (!isSuccess)
                    {
                        var messageProp = responseType.GetProperty("Message");
                        string message = messageProp?.GetValue(objectResult.Value)?.ToString() ?? "Unknown reason";
                        
                        var errorProp = responseType.GetProperty("Error");
                        var errorObj = errorProp?.GetValue(objectResult.Value);
                        
                        string detailsStr = string.Empty;
                        if (errorObj != null)
                        {
                            var detailsProp = errorObj.GetType().GetProperty("Details");
                            var detailsVal = detailsProp?.GetValue(errorObj);
                            if (detailsVal != null)
                            {
                                detailsStr = " Details: " + JsonSerializer.Serialize(detailsVal);
                            }
                        }

                        // Log Information that user request failed due to business logic (validation, not found, forbidden, etc.)
                        _logger.LogInformation(
                            "Request to {Method} {Path} failed. Reason: {Reason}.{Details}",
                            context.HttpContext.Request.Method,
                            context.HttpContext.Request.Path,
                            message,
                            detailsStr);
                    }
                }
            }

            await next();
        }
    }
}
