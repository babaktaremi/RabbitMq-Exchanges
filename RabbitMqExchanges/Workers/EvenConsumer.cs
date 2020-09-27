using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqExchanges.Configuration;
using RabbitMqExchanges.Model;
using RabbitMqExchanges.Settings;

namespace RabbitMqExchanges.Workers
{
   public class EvenConsumer:BackgroundService
    {
        private readonly ILogger<EvenConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly QueueSettings _queueSettings;

        public EvenConsumer(ILogger<EvenConsumer> logger, IServiceProvider serviceProvider, IOptions<QueueSettings> queueSettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _queueSettings = queueSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               using var scope = _serviceProvider.CreateScope();

                var rabbitMq = scope.ServiceProvider.GetRequiredService<RabbitMqService>();

                rabbitMq.Channel.QueueDeclare(_queueSettings.OddQueue, _queueSettings.IsDurable, false,
                    _queueSettings.AutoDelete, null);

                var consumer = rabbitMq.CreateConsumer();

                consumer.Received += (sender, args) =>
                {
                    var body = args.Body;
                    var messageString = Encoding.UTF8.GetString(body.ToArray());
                    var message = JsonSerializer.Deserialize<MessageViewModel>(messageString);

                    _logger.LogWarning($"Message Received Number {message.Message} at {message.SentDate}");
                };

                rabbitMq.Channel.BasicConsume(queue: _queueSettings.EvenQueue, autoAck: true, consumer: consumer);
             
                await Task.Delay(200, stoppingToken);
            }
        }
    }
}
