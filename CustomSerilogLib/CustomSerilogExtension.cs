using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;

namespace CustomSerilogLib;

public static class CustomSerilogExtension
{
    public static IServiceCollection AddCustomSerilog(this IServiceCollection serviceCollection)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(a => a.Sink(new CustomJsonSink()))
            .CreateLogger();
        
        serviceCollection.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });
        return serviceCollection;
    }
}