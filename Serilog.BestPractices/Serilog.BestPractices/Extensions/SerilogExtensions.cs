using Elastic.CommonSchema.Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace Serilog.BestPractices.Extensions;

public static class SerilogExtension
{
	public static void AddSerilogApi(this WebApplicationBuilder builder, IConfiguration configuration)
	{
		var logLevel = builder.Environment.IsProduction() ? LogEventLevel.Warning : LogEventLevel.Debug;

		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
			.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
			.Filter.ByExcluding(z => z.MessageTemplate.Text.Contains("Business error"))
			.Enrich.FromLogContext()
			.Enrich.WithExceptionDetails()
			.Enrich.WithCorrelationId()
			.Enrich.WithCorrelationIdHeader()
			.Enrich.WithProperty("ApplicationName", "API Serilog")
			.WriteTo.Async(wt => wt.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Properties:j}{NewLine}{Exception}", restrictedToMinimumLevel: logLevel))
			.WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
			{
				CustomFormatter = new EcsTextFormatter(),
				AutoRegisterTemplate = true,
				IndexFormat = "indexlogs",
				ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"], configuration["ElasticsearchSettings:password"])
			}))
			.CreateLogger();

		builder.Logging.ClearProviders();
		builder.Host.UseSerilog(Log.Logger, true);
	}
}