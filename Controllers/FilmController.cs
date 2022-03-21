using Microsoft.AspNetCore.Mvc;
using swi2grupp1WebAPI.MessageHandler;

namespace swi2grupp1WebAPI.Controllers
{    /*
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

    public async Task<IEnumerable<Film>> Get(string? name = "")
    {
        Film[] films = await new MessageSender().GetFilmAsync(name);
        return films;
    }
    /*
     * POST-Methode
     * erwartet ein WeatherForecast-JSON-Objekt
     * Dieses wird an den Azure Service Bus zur Weiterverarbeitung gesendet
     */
    [HttpPost(Name = "PostFilm")]
    //public void Post([FromBody]WeatherForecast weatherForecast)
    public async void Post(Film film)
    {
        // Objekt versenden
        int statusCode = await new MessageSender().SendFilmAsync(film);
    }
}
}