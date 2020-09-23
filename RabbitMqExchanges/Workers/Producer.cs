using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMqExchanges.Model;
using RabbitMqExchanges.Services;

namespace RabbitMqExchanges.Workers
{
   public class Producer:BackgroundService
   {
       private readonly ILogger<Producer> _logger;
       private readonly IServiceProvider _serviceProvider;

       public Producer(ILogger<Producer> logger, IServiceProvider serviceProvider)
       {
           _logger = logger;
           _serviceProvider = serviceProvider;
       }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var scope = _serviceProvider.CreateScope();

                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

                Random rd = new Random();

                int number = rd.Next(100_000, 200_000);

                await messageService.PublishMessage(new MessageViewModel
                    {Message = number.ToString(), SentDate = DateTime.Now});

                _logger.LogInformation($"number published {number}");

                await Task.Delay(10_00, stoppingToken);
            }
        }
    }
}
