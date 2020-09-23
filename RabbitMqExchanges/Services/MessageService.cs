using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqExchanges.Configuration;
using RabbitMqExchanges.Model;
using RabbitMqExchanges.Settings;

namespace RabbitMqExchanges.Services
{
   public class MessageService:IMessageService
   {
       private readonly RabbitMqService _rabbitMqService;
       private readonly ExchangeSettings _exchangeSettings;
       private readonly QueueSettings _queueSettings;
       private readonly ILogger<MessageService> _logger;
       public MessageService(RabbitMqService rabbitMqService,IOptions<ExchangeSettings> exchangeSettings,IOptions<QueueSettings> queueSettings, ILogger<MessageService> logger)
       {
           _rabbitMqService = rabbitMqService;
           _logger = logger;
           _exchangeSettings = exchangeSettings.Value;
           _queueSettings = queueSettings.Value;
       }

       public  Task PublishMessage(MessageViewModel model)
       {
           _rabbitMqService.Channel.ExchangeDeclare(_exchangeSettings.ExchangeName,ExchangeType.Direct,_exchangeSettings.IsDurable,_exchangeSettings.AutoDelete,null);

            //Declaring Two Queues for consuming even or odd numbers

            _rabbitMqService.Channel.QueueDeclare(_queueSettings.EvenQueue, _queueSettings.IsDurable, false,
                _queueSettings.AutoDelete, null);

            _rabbitMqService.Channel.QueueDeclare(_queueSettings.OddQueue, _queueSettings.IsDurable, false,
                _queueSettings.AutoDelete, null);

            _rabbitMqService.Channel.QueueBind(_queueSettings.OddQueue,_exchangeSettings.ExchangeName,"odd");
            _rabbitMqService.Channel.QueueBind(_queueSettings.EvenQueue,_exchangeSettings.ExchangeName,"even");

            

            var number = int.Parse(model.Message);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model));
            if(number % 2 == 0) {
                _rabbitMqService.Channel.BasicPublish(exchange: _exchangeSettings.ExchangeName, routingKey: "even",mandatory:false, basicProperties: null, body: body);
                _rabbitMqService.Channel.Close();
                return Task.CompletedTask;
            }
            _rabbitMqService.Channel.BasicPublish(exchange: _exchangeSettings.ExchangeName, routingKey: "odd", mandatory: false, basicProperties: null, body: body);
            
            return Task.CompletedTask;

        }
    }
}
