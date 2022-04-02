using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
using System.Text;

namespace swi2grupp1WebAPI.MessageHandler
{
    public class MessageSenderAndReceiver
    {
        // AzureBus-Connectionsstring und Queue, siehe appsettings.json, program.cs und AppConfiguration.cs
        string conn;
        string queue;
        string responseQueue;

        public MessageSenderAndReceiver()
        {
            this.conn = AppConfiguration.AzureServiceBusConnectionString;
            this.queue = AppConfiguration.AzureServiceBusRequests;
            this.responseQueue = AppConfiguration.AzureServiceBusResponse;
        }

        // Get bearbeiten: Parameter als Command-Objekt an Azure Service Bus als Sessionless-Message senden
        // die gefundenen Datenobjekte als Json-Objekte mit einer Session-Message erhalten
        public async Task<Film[]> SendGetFilmAsync(int id, string name)
        {
            // Befehlsobjekt erstellen
            Command befehl = new Command();
            befehl.Befehl = "Read";
            befehl.Id = id;
            befehl.Name = name;
            string sendString = JsonConvert.SerializeObject(befehl);
            // Client mit der Connection zum Azure Service Bus
            ServiceBusClient client = new ServiceBusClient(this.conn);
            // Senderobjekt mit dem Warteschlangennamen
            ServiceBusSender sender = client.CreateSender(this.queue);

            // Message erstellen
            var replyToSessionId = Guid.NewGuid().ToString();
            ServiceBusMessage message = new ServiceBusMessage(sendString)
            {
                SessionId = Guid.NewGuid().ToString(),
                ReplyToSessionId = replyToSessionId,
                ReplyTo = this.responseQueue,
                ContentType = "application/json",
            };

            // Message senden
            await sender.SendMessageAsync(message);
            await sender.DisposeAsync();

            // hier die Reply-Message erhalten als Session-Message in gewünschter Queue erhalten
            Film[] filmListe;
            ServiceBusSessionReceiver receiver = await client.AcceptSessionAsync(this.responseQueue, replyToSessionId);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(receivedMessage.Body);
                Console.WriteLine("Wieder erhaltener Text: " + text);
                filmListe = JsonConvert.DeserializeObject<Film[]>(text);
            }
            else
            {
                filmListe = new Film[0];
            }
            await receiver.DisposeAsync();
            await client.DisposeAsync();

            return filmListe;
        }

        // POST und PUT bearbeiten: Parameter als WeatherForecast-Objekt an Azure Service Bus als Sessionless-Message senden
        // das erstellte/geänderte Datenobjekt als Json-Objekte mit einer Session-Message erhalten
        public async Task<Film> SendFilmAsync(Film film)
        {
            // Datenobjekt serialisieren
            var jsonString = JsonConvert.SerializeObject(film);
            // Client mit der Connection zum Azure Service Bus
            ServiceBusClient client = new ServiceBusClient(this.conn);
            // Senderobjekt mit dem Warteschlangennamen
            ServiceBusSender sender = client.CreateSender(this.queue);
            // Batch erstellen
            ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            // Message erstellen
            var replyToSessionId = Guid.NewGuid().ToString();
            ServiceBusMessage message = new ServiceBusMessage(jsonString)
            {
                SessionId = Guid.NewGuid().ToString(),
                ReplyToSessionId = replyToSessionId,
                ReplyTo = this.responseQueue,
                ContentType = "application/json",
            };

            // Message anhängen
            if (messageBatch.TryAddMessage(message))
            {
                try
                {
                    // hier Message senden
                    await sender.SendMessagesAsync(messageBatch);
                }
                finally
                {
                    await sender.DisposeAsync();
                }
            }
            // hier die Retour-Message erhalten
            Film movie;
            ServiceBusSessionReceiver receiver = await client.AcceptSessionAsync(this.responseQueue, replyToSessionId);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(receivedMessage.Body);
                Console.WriteLine("Wieder erhaltener Text: " + text);
                movie = JsonConvert.DeserializeObject<Film>(text);
            }
            else
            {
                movie = new Film();
            }
            await receiver.DisposeAsync();
            await client.DisposeAsync();

            return movie;
        }

        // Delete bearbeiten: Parameter als Command-Objekt an Azure Service Bus als Sessionless-Message senden
        // das gelöschte Datenobjekt als Json-Objekt mit einer Session-Message erhalten
        public async Task<Film> SendDeleteFilmAsync(int id)
        {
            // Client mit der Connection zum Azure Service Bus
            ServiceBusClient client = new ServiceBusClient(this.conn);
            // Senderobjekt mit dem Warteschlangennamen
            ServiceBusSender sender = client.CreateSender(this.queue);
            // Batch erstellen
            ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            // Befehlsobjekt erstellen
            Command befehl = new Command();
            befehl.Befehl = "Delete";
            befehl.Id = id;
            string sendString = JsonConvert.SerializeObject(befehl);
            // Message erstellen
            var replyToSessionId = Guid.NewGuid().ToString();
            ServiceBusMessage message = new ServiceBusMessage(sendString)
            {
                SessionId = Guid.NewGuid().ToString(),
                ReplyToSessionId = replyToSessionId,
                ReplyTo = this.responseQueue,
                ContentType = "application/json",
            };

            // Message anhängen
            if (messageBatch.TryAddMessage(message))
            {
                try
                {
                    // hier Message senden
                    await sender.SendMessagesAsync(messageBatch);
                }
                finally
                {
                    await sender.DisposeAsync();
                }
            }
            // hier die Retour-Message erhalten
            Film movie;
            ServiceBusSessionReceiver receiver = await client.AcceptSessionAsync(this.responseQueue, replyToSessionId);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(receivedMessage.Body);
                Console.WriteLine("Wieder erhaltener Text: " + text);
                movie = JsonConvert.DeserializeObject<Film>(text);
            }
            else
            {
                movie = new Film();
            }
            await receiver.DisposeAsync();
            await client.DisposeAsync();

            return movie;
        }
    }
}
