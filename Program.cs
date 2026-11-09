using System.Transactions;
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
await sender.SendMessageAsync(new ServiceBusMessage("learning about transactions"));

// Create receiver
ServiceBusReceiver receiver = client.CreateReceiver("firstqueue");
ServiceBusReceivedMessage recvMsg = await receiver.ReceiveMessageAsync();

using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
{
    await sender.SendMessageAsync(new ServiceBusMessage("Inside Transactions"));
    await receiver.CompleteMessageAsync(recvMsg);

    ts.Complete();
}

// for (int i = 0; i < 4; i++)
// {
//     ServiceBusReceivedMessage recmsg = await receiver.ReceiveMessageAsync();
// }
Console.ReadLine();
