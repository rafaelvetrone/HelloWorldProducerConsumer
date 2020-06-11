using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorldProducerConsumer
{
    public class HelloWorldConsumer : IHostedService
    {
        private readonly ILogger _logger;
        public HelloWorldConsumer(ILogger<HelloWorldConsumer> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<int, string>(conf).Build())
            {                
                c.Subscribe("fila_hello_world");                          
                var cts = new CancellationTokenSource();

                try
                {
                    while (true)
                    {
                        var message = c.Consume(cts.Token);

                        bool success = true;
                        var settings = new JsonSerializerSettings
                        {
                            Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; }
                        };
                        MessageDto messageObj = JsonConvert.DeserializeObject<MessageDto>(message.Message.Value, settings);
                        
                        int ident = message.Message.Key;
                        
                        _logger.LogInformation("Mensagem: " + messageObj.Message);
                        _logger.LogInformation("ApplicationId: " + messageObj.ApplicationId);
                        _logger.LogInformation("RequestId: " + messageObj.RequestId);
                        _logger.LogInformation("Timestamp: " + messageObj.Timestamp);
                        _logger.LogInformation("Kafka Key: " + ident);

                    }
                }
                catch (OperationCanceledException)
                {
                    c.Close();
                }
            }

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

