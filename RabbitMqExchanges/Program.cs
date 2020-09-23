using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMqExchanges.Configuration;
using RabbitMqExchanges.Services;
using RabbitMqExchanges.Settings;
using RabbitMqExchanges.Workers;

namespace RabbitMqExchanges
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services.Configure<ExchangeSettings>(configuration.GetSection(nameof(ExchangeSettings)));
                    services.Configure<QueueSettings>(configuration.GetSection(nameof(QueueSettings)));

                    services.AddRabbitMq("localhost");

                    services.AddScoped<IMessageService, MessageService>();

                    services.AddHostedService<OddConsumer>();
                    services.AddHostedService<EvenConsumer>();
                    services.AddHostedService<Producer>();
                });
    }
}
