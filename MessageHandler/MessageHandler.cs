using Newtonsoft.Json;
using System.Text;

namespace swi2grupp1WebAPI.MessageHandler
{

    public class MessageHandler
    {
        // für Testdaten
        private static readonly string[] Names = new[]
        {
        "Spiderman", "Ironman", "Ironman 2", "Project X", "Amazing Spiderwoman", "Sting 3", "Titanic: Return of Jack", "Avatar 2 (2033)"
        };

        private static readonly string[] Feelings = new[]
        {
        "langweilig", "spannend", "bin eingeschlafen", "romantisch", "actionreich", "lustig"
        };


        /*
         * sendet Message an Azure Service Bus
         * erwartet eine Antwort und gibt diese zurück
         */

        public async Task<Film[]> GetFilmAsync(int? id, string? name)
        {
            id = id == null ? 0 : id;
            name = name == null ? "Guardians of the Galaxy" : name;
            name = name.Trim() == "" ? "Guardians of the Galaxy" : name;

            Console.WriteLine("hier würde die Nachricht an den Azure Service Bus gesendet.." +
                           "um aktuelle Daten zu holen: ev. Ort " + name);

            // generierte Testdaten
            int anz = id == 0 ? 10 : 1;
            return Enumerable.Range(1, anz).Select(index => new Film
            {
                Id = (int)((id == 0) ? index : id),
                Name = Names[Random.Shared.Next(Names.Length)],
                Rating = Random.Shared.Next(0, 10),
                Feeling = Feelings[Random.Shared.Next(Feelings.Length)]
            })
            .ToArray();
        }

        /*
         * sendet Message an Azure Service Bus
         * Gibt Status-Code zurück: 0 = ok
         */
        public async Task<int> CreateFilmAsync(Film film)
        {
            Console.WriteLine("hier würde die Nachricht an den Azure Service Bus gesendet fürs Erstellen.." +
                film.Id + " " + film.Name + " " +
                film.Rating + " " +
                film.Feeling);
            return 0;
        }
        public async Task<int> UpdateFilmAsync(Film film)
        {
            Console.WriteLine("hier würde die Nachricht an den Azure Service Bus gesendet fürs Update.." +
                film.Id + " " + film.Name + " " +
                film.Rating + " " +
                film.Feeling);
            return 0;
        }
        public async Task<int> DeleteFilmAsync(int Id)
        {
            Console.WriteLine("hier würde die Nachricht an den Azure Service Bus gesendet, um zu Löschen.." + Id);

            return 0;
        }
    }
}