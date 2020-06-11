using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Confluent.Kafka;

using System;
using System.Collections;
using System.Text;
using System.Security.Policy;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace HelloWorldProducerConsumer
{
    public class TimedHelloWorldProducer : IHostedService, IDisposable
    {        
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private Timer _timer;
        public TimedHelloWorldProducer(ILoggerFactory loggerFactory, IConfiguration configuration)
        {            
            _configuration = configuration;
            this._logger = loggerFactory.CreateLogger<TimedHelloWorldProducer>();            
        }        

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");                        

            _timer = new Timer(DoWork, null, 1000, 5000);            

            await Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                _logger.LogInformation("Timed Background Service is working.");

                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

                using (var producer = new ProducerBuilder<int, string>(config).Build())
                {
                    try
                    {                                                
                        MessageDto message = new MessageDto()
                        {
                            ApplicationId = Process.GetCurrentProcess().Id,
                            Message = "Hello World",
                            RequestId = Guid.NewGuid().ToString(),
                            Timestamp = DateTime.Now
                        };
                        
                        var sendResult = producer
                                            .ProduceAsync("fila_hello_world", new Message<int, string> { Key = new Random().Next(int.MaxValue), Value =  JsonConvert.SerializeObject(message) })
                                                .GetAwaiter()
                                                    .GetResult();                        
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to Kafka");
            }
        }        

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
