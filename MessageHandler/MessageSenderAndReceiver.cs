using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using swi2grupp1WebAPI.Data;
using System.Drawing;
//using Microsoft.ServiceBus.Messaging;
using System.Text;

namespace swi2grupp1WebAPI.MessageHandler
{
    public class MessageSenderAndReceiver
    {
        // AzureBus-Connectionsstring und Queue, siehe appsettings.json, program.cs und AppConfiguration.cs
        string conn;
        string queueCommand;
        string queueQuery;
        string responseQueue;
        ConvertFilm convMovie;

        public MessageSenderAndReceiver()
        {
            this.conn = AppConfiguration.AzureServiceBusConnectionString;

            this.queueCommand = AppConfiguration.AzureServiceBusCommand;
            this.queueQuery = AppConfiguration.AzureServiceBusQuery;

            this.responseQueue = AppConfiguration.AzureServiceBusResponse;
            this.convMovie = new ConvertFilm();
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
            ServiceBusSender sender = client.CreateSender(this.queueQuery);

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
            FilmMsg[] movieListemsg;
            Film[] movieListe;
            ServiceBusSessionReceiver receiver = await client.AcceptSessionAsync(this.responseQueue, replyToSessionId);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(receivedMessage.Body);
                //Console.WriteLine("Wieder erhaltener Text: " + text);
                // erhaltene Liste in einen Objekt-Array konvertieren
                movieListemsg = JsonConvert.DeserializeObject<FilmMsg[]>(text);

                // den Objekt-Array von WeatherForecastMsg in WeatherForecast konvertieren
                if (movieListemsg == null)
                {
                    movieListemsg = new FilmMsg[0];
                }
                movieListe = new Film[movieListemsg.Length];
                for (int i = 0; i < movieListemsg.Length; i++)
                {
                    movieListe[i] = convMovie.GetFilmFromMsg(movieListemsg[i]);
                }
                await receiver.CompleteMessageAsync(receivedMessage);
                Console.WriteLine("Erhaltene Objekte: " + movieListemsg.Length);
            }
            else
            {
                movieListe = new Film[0];
            }

            await receiver.DisposeAsync();
            await client.DisposeAsync();

            return movieListe;
        }

        // POST und PUT bearbeiten: Parameter als WeatherForecast-Objekt an Azure Service Bus als Sessionless-Message senden
        // das erstellte/geänderte Datenobjekt als Json-Objekte mit einer Session-Message erhalten
        public async Task<Film> SendFilmAsync(Film film)
        {

            // Bilder zwischenspeichern, später dann unten in CosmosDB speichern
            String bild = film.Bild;
            String vorschau = film.Vorschau;

            // Datenobjekt serialisieren, Objekt noch konvertieren
            var jsonString = JsonConvert.SerializeObject(convMovie.GetFilmMsg(film));
            // Client mit der Connection zum Azure Service Bus
            ServiceBusClient client = new ServiceBusClient(this.conn);
            // Senderobjekt mit dem Warteschlangennamen
            ServiceBusSender sender = client.CreateSender(this.queueCommand);
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
            FilmMsg moviemsg;
            ServiceBusSessionReceiver receiver = await client.AcceptSessionAsync(this.responseQueue, replyToSessionId);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(receivedMessage.Body);
                //Console.WriteLine("Wieder erhaltener Text: " + text);
                moviemsg = JsonConvert.DeserializeObject<FilmMsg>(text);
                Console.WriteLine("Erhaltene Id: " + moviemsg.Id);
                await receiver.CompleteMessageAsync(receivedMessage);
            }
            else
            {
                moviemsg = new FilmMsg();
            }
            Film movie = convMovie.GetFilmFromMsg(moviemsg);
            // ursprüngliche Bilder wieder hinzufügen
            movie.Bild = bild;
            movie.Vorschau = vorschau;

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
            ServiceBusSender sender = client.CreateSender(this.queueCommand);
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
            FilmMsg moviemsg;
            ServiceBusSessionReceiver receiver = await client.AcceptSessionAsync(this.responseQueue, replyToSessionId);
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(receivedMessage.Body);
                //Console.WriteLine("Wieder erhaltener Text: " + text);
                moviemsg = JsonConvert.DeserializeObject<FilmMsg>(text);
                Console.WriteLine("Erhaltene Id: " + moviemsg.Id);
                await receiver.CompleteMessageAsync(receivedMessage);
            }
            else
            {
                moviemsg = new FilmMsg();
            }
            Film movie = convMovie.GetFilmFromMsg(moviemsg);
            await receiver.DisposeAsync();
            await client.DisposeAsync();

            return movie;
        }
    }
}
