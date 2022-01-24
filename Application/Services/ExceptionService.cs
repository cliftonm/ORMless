using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace Demo
{
    // Borrowed from here: http://netitude.bc3tech.net/2017/07/31/using-middleware-to-trap-exceptions-in-asp-net-core/
    // Note that middleware exception handling is different from exception filters:
    // https://damienbod.com/2015/09/30/asp-net-5-exception-filters-and-resource-filters/
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.2#exception-filters
    // Exception filters do NOT catch exceptions that occur in the middleware.
    public class MiddlewareExceptionHandler
    {
        private readonly RequestDelegate _next;

        public MiddlewareExceptionHandler(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            // This handles the problem when the AUTHORIZATION token doesn't actually validate and ASP.NET Core middleware generates this:
            // An unhandled exception occurred while processing the request.
            // InvalidOperationException: No authenticationScheme was specified, and there was no DefaultChallengeScheme found.
            // We want to handle this error as a "not authorized" response.
            catch (InvalidOperationException)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync("{\"status\":401,\"message\":\"Not authorized.\"}");
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var exReport = new ExceptionReport(ex);
                var exJson = JsonConvert.SerializeObject(exReport, Formatting.Indented);
                await context.Response.WriteAsync(exJson);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExceptionExtensions
    {
        public static IApplicationBuilder UseHttpStatusCodeExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MiddlewareExceptionHandler>();
        }
    }

    public static class ExceptionReportExtensionMethods
    {
        public static ExceptionReport CreateReport(this Exception ex)
        {
            return new ExceptionReport(ex);
        }

        public static T[] Drop<T>(this T[] items, int n = 0)
        {
            // We could use C# 8's ^ operator to take all but the last n...
            return items.Take(items.Length - (1 + n)).ToArray();
        }
    }

    public class ExceptionReport
    {
        public DateTime When { get; } = DateTime.Now;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationMessage { get; set; }

        public string ExceptionMessage { get; set; }

        public List<StackFrameData> CallStack { get; set; } = new List<StackFrameData>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExceptionReport InnerException { get; set; }

        public ExceptionReport(Exception ex, int exceptLastN = 0)
        {
            ExceptionMessage = ex.Message;
            var st = new StackTrace(ex, true);
            var frames = st.GetFrames()?.Drop(exceptLastN) ?? new StackFrame[0];
            CallStack.AddRange(
                frames
                    .Where(frame => !String.IsNullOrEmpty(frame.GetFileName()))
                    .Select(frame => new StackFrameData(frame)));
            InnerException = ex.InnerException?.CreateReport();
        }
    }

    public class StackFrameData
    {
        public string FileName { get; private set; }
        public string Method { get; private set; }
        public int LineNumber { get; private set; }

        public StackFrameData(StackFrame sf)
        {
            FileName = sf.GetFileName();
            Method = sf.GetMethod().Name;
            LineNumber = sf.GetFileLineNumber();
        }

        public override string ToString()
        {
            return $"File: {FileName}\r\nMethod: {Method}\r\nLine: {LineNumber}";
        }
    }
}
