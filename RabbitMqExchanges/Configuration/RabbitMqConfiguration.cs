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

    public class RabbitMqService:IDisposable
    {
        private RabbitMqSetting _settings;
        public IModel Channel { get; private set; }
        public IConnection Connection { get; private set; }
        public RabbitMqService(IOptions<RabbitMqSetting> options)
        {
            _settings = options.Value;

            var factory = new ConnectionFactory() { HostName = _settings.RabbitMqUrl };
             Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            // Consumer= new EventingBasicConsumer(this.Channel);
        }

        public AsyncEventingBasicConsumer CreateConsumerAsync()
        {
            return new AsyncEventingBasicConsumer(Channel);
        }

        public EventingBasicConsumer CreateConsumer()
        {
            return new EventingBasicConsumer(Channel);
        }

        public void Dispose()
        {
            if(!Channel.IsClosed)
                Channel.Close();
            Channel?.Dispose();
            Connection.Close();
            Connection.Dispose();
            
        }
    }
}