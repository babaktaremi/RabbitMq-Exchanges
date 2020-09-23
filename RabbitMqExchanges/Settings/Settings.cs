using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMqExchanges.Settings
{
    public class ExchangeSettings
    {
        public string ExchangeName { get; set; }
        public bool AutoDelete { get; set; }
        public bool IsDurable { get; set; }

    }

    public class QueueSettings
    {
        public string OddQueue { get; set; }
        public string EvenQueue { get; set; }
        public bool IsDurable { get; set; }
        public bool AutoDelete { get; set; }
    }
}
