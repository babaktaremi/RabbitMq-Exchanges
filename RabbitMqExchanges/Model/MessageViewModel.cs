using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMqExchanges.Model
{
   public class MessageViewModel
    {
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
    }
}
