using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMqExchanges.Model;

namespace RabbitMqExchanges.Services
{
   public interface IMessageService
   {
       Task PublishMessage(MessageViewModel model); //can be any type of object passed to the queue
   }
}
