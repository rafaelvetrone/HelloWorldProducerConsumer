using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HelloWorldProducerConsumer
{
    public class MessageDto
    {
        public string Message { get; set; }
        public int ApplicationId { get; set; }
        public string RequestId { get; set; }

        public DateTime Timestamp { get; set; }        
    }
}
