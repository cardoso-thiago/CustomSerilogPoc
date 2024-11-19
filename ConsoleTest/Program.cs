using CustomSerilogLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => { services.AddCustomSerilog(); })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Teste de log \n \t \r \f \\n \\t \\r \\f \" com {@Data}", new { Name = "\nUser", Age = 25 });

host.Run();