using Serilog;
using Serilog.BestPractices.Extensions;
using Serilog.BestPractices.Middlewares;

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.AddSerilogApi(builder.Configuration);

	builder.Services.AddControllers();

	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	var app = builder.Build();
	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseMiddleware<ErrorHandlingMiddleware>();
	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}
