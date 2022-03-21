using Newtonsoft.Json;
using System.Text;

namespace swi2grupp1WebAPI.MessageHandler
{
        
    public class MessageSender
    {
        // test data (only film names)
        private static readonly string[] Names = new[]
        {
        "Spiderman", "Ironman", "Ironman 2", "Project X", "Amazing Spiderwoman", "Sting 3", "Titanic: Return of Jack", "Avatar 2 (2033)"
        };

        /*
         * sendet Message an Azure Service Bus
         * erwartet eine Antwort und gibt diese zurück
         */

        public async Task<Film[]> GetFilmAsync(string? name)
        {
            name  = name == null ? string.Empty : name;

            Console.WriteLine("hier würde die Nachricht an den Azure Service Bus gesendet.." +
                           "um aktuelle Daten zu holen: ev. Ort " + name);

            Console.WriteLine("da wird auf Antwort von Azure Service Bus gewartet ...");

            // generierte Testdaten
            int anz = name.Trim() == "" ? 1 : 10;
            Console.WriteLine("gibt Anzahl zurück: " + anz);
            return Enumerable.Range(1, anz).Select(index => new Film
            {
                Picture = "Beispielbild",
                ID = 1,
                Name = Names[Random.Shared.Next(Names.Length)]
            })
            .ToArray();
        }

        /*
         * sendet Message an Azure Service Bus
         * Gibt Status-Code zurück: 0 = ok
         */
        public async Task<int> SendFilmAsync(Film film)
        {
            Console.WriteLine("hier würde die Nachricht an den Azure Service Bus gesendet.." +
                film.Picture + " " + film.Name + " " + film.ID);
            return 0;
        }
    }
}