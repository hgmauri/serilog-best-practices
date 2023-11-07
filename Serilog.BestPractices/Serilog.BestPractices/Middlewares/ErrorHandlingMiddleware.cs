using Serilog.Context;
using System.Net;

namespace Serilog.BestPractices.Middlewares;

public class ErrorHandlingMiddleware
{
	private readonly RequestDelegate _next;

	public ErrorHandlingMiddleware(RequestDelegate next)
	{
		this._next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			using (LogContext.PushProperty("UserName", context?.User?.Identity?.Name ?? "anônimo"))
			{
				await _next(context);
			}
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		Log.Error(exception, "Error");

		var code = HttpStatusCode.InternalServerError;

		var result = System.Text.Json.JsonSerializer.Serialize(new { error = exception?.Message });

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)code;
		return context.Response.WriteAsync(result);
	}
}