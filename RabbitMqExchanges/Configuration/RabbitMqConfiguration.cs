using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqExchanges.Configuration
{
    public static class RabbitMqExtensions
    {
        public static void AddRabbitMq(this IServiceCollection services, string url)
        {
            services.Configure<RabbitMqSetting>(r => r.RabbitMqUrl = url);
            services.AddScoped<RabbitMqService>();
        }
    }


    public class RabbitMqSetting
    {
        public string RabbitMqUrl { get; set; }
    }

    public class RabbitMqService
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private EventingBasicConsumer _consumer;
        private AsyncEventingBasicConsumer _asyncConsumer;

        public IModel Channel { get; private set; }
        public RabbitMqService(IOptions<RabbitMqSetting> options)
        {
            var settings = options.Value;

            if (_factory == null)
                _factory = new ConnectionFactory() { HostName = settings.RabbitMqUrl };

            if (_connection == null)
                _connection = _factory.CreateConnection();

         
            if(Channel==null)
                Channel = _connection.CreateModel();

            // Consumer= new EventingBasicConsumer(this.Channel);
        }

        public AsyncEventingBasicConsumer CreateConsumerAsync()
        {
            return _asyncConsumer ??= new AsyncEventingBasicConsumer(Channel);
        }

        public EventingBasicConsumer CreateConsumer()
        {
            return _consumer ??= new EventingBasicConsumer(Channel);
        }

    }
}