using Microsoft.AspNetCore.Mvc;
using swi2grupp1WebAPI.MessageHandler;
using swi2grupp1WebAPI.DataAccessLayer;
using swi2grupp1WebAPI.Data;
using System.Collections.Generic;

namespace swi2grupp1WebAPI.Controllers
{
    /*
     * Controller für REST-Services
     * Alle Aufrufe sind asynchron programmiert
     * GET
     * POST
     */
    [ApiController]
    [Route("[controller]")]
    public class FilmController : ControllerBase
    {


        private readonly ILogger<FilmController> _logger;
        public FilmController(ILogger<FilmController> logger)
        {
            _logger = logger;
        }

        /*
         * GET-Methode
         * Gibt ein Array von WeatherForecast-Objekten
         * kann mit einem Parameter Ort gefiltert werden
         */

        [HttpGet(Name = "GetFilm")]

        public async Task<IEnumerable<Film>> Get(int? id = 0, string? name = "")
        {
            Console.WriteLine("REST-API: Get-Request: " + id + " " + name);
            // über Azure Service Bus die Daten in der DB updaten
            Film[] films = await new MessageSenderAndReceiver().SendGetFilmAsync((int)id, name); ;
            // nur für das erste Objekt (sofern vorhanden), Bild lesen und hinzufügen
            if (films.Length > 0)
            {
                FilmImage moviei = await new CosmosDAC().GetImage(films[0].Id.ToString());
                if (moviei.Id == films[0].Id.ToString())
                {
                    films[0].Bild = moviei.Image;
                    films[0].Vorschau = moviei.Preview;
                }
            }
            return films;
        }

        /*
         * POST-Methode - neues Objekt aufnehmen
         * erwartet ein WeatherForecast-JSON-Objekt
         * Dieses wird an den Azure Service Bus zur Weiterverarbeitung gesendet
         */
        [HttpPost(Name = "PostFilm")]
        public async Task<Film> Post(Film film)
        {
            Console.WriteLine("REST-API: Post-Request: " + film.Id);
            // Datenobjekt an Azure Service Bus senden
            Film movie = await new MessageSenderAndReceiver().SendFilmAsync(film);
            // Bild noch an Cosmos DB senden
            if (movie.Bild != null)
            {
                FilmImage moviei = new FilmImage();
                moviei.Id = movie.Id.ToString();
                moviei.Image = movie.Bild;
                moviei.Preview = movie.Vorschau;
                await new CosmosDAC().AddImage(moviei);
            }
            return movie;
        }

        /*
        * PUT-Methode - bestehendes Objekt ändern
        * erwartet ein WeatherForecast-JSON-Objekt
        * Dieses wird an den Azure Service Bus zur Weiterverarbeitung gesendet
        */
        [HttpPut(Name = "PutFilm")]
        public async Task<Film> Put(Film film)
        {
            Console.WriteLine("REST-API: Put-Request: " + film.Id);
            // Datenobjekt an Azure Service Bus senden
            Film movie = await new MessageSenderAndReceiver().SendFilmAsync(film);
            // Bild noch an Cosmos DB senden
            if (movie.Bild != null)
            {
                FilmImage moviei = new FilmImage();
                moviei.Id = movie.Id.ToString();
                moviei.Image = movie.Bild;
                moviei.Preview = movie.Vorschau;
                await new CosmosDAC().ReplaceImage(moviei);
            }
            return movie;
        }

        /*
        * DELETE-Methode - bestehendes Objekt löschen
         * erwartet ein WeatherForecast-JSON-Objekt
         * Dieses wird an den Azure Service Bus zur Weiterverarbeitung gesendet
        */
        [HttpDelete(Name = "DeleteFilm")]
        //DeleteFilmForecast
        public async Task<Film> Delete(int id)
        {
            Console.WriteLine("REST-API: Delete-Request: " + id);
            // Löschanweisung an Azure Service Bus senden
            Film movie = await new MessageSenderAndReceiver().SendDeleteFilmAsync(id);
            // Löschanweisung an CosmosDB senden
            if (movie.Id > 0)
            {
                await new CosmosDAC().DeleteImage(movie.Id.ToString());
            }
            return movie;
        }
    }
}