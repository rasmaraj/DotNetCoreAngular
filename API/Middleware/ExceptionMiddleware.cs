using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next { get; set; }
        private ILogger<ExceptionMiddleware> _logger { get; set; }
        private IHostEnvironment _env { get; set; }
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this._next = next;
            this._logger = logger;
            this._env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = _env.IsDevelopment() ?
                    new API.Errors.ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new Errors.ApiException(context.Response.StatusCode, "Internal server error");

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                var json=JsonSerializer.Serialize(response,options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}