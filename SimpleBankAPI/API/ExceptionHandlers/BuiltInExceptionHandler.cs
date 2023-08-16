using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace SimpleBankAPI.API.ExceptionHandlers;

public static class BuiltInExceptionHandler
{
    public static void AddErrorHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature is not null)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Something went wrong"
                    }));
                }
            }));
    }
}