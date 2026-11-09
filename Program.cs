using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var connectionString = config["ServiceBus:ConnectionString"];

// Connection
await using var client = new ServiceBusClient(connectionString);

// Create sender
ServiceBusSender sender = client.CreateSender("firstqueue");

// Send message
await sender.SendMessageAsync(new ServiceBusMessage("learning about queues"));

// Create processor
await using ServiceBusProcessor serviceBusProcessor = client.CreateProcessor(
    "firstqueue",
    new ServiceBusProcessorOptions { AutoCompleteMessages = true, MaxConcurrentCalls = 1 }
);

// Configure message
serviceBusProcessor.ProcessMessageAsync += MessageHandler;
serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

// Configure handler
async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
{
    // Process your message
    Console.WriteLine(processMessageEventArgs.Message.Body.ToString());
}

// Configure error
Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
{
    Console.WriteLine(processErrorEventArgs.ErrorSource);
    return Task.CompletedTask;
}

// Start processing
await serviceBusProcessor.StartProcessingAsync();
Console.ReadLine();
