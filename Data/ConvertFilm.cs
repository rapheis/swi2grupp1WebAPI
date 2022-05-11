namespace swi2grupp1WebAPI.Data
{
    public class ConvertFilm
    {

        /*
         * Konvertiert WeatherForecast in ein WeatherForecastMsg-Objekt
         */
        public FilmMsg GetFilmMsg(Film movie)
        {
            FilmMsg moviemsg = new FilmMsg();
            moviemsg.Id = movie.Id;
            moviemsg.Name = movie.Name;
            moviemsg.Rating = movie.Rating;
            moviemsg.Feeling = movie.Feeling;
            // Bild wird separat gespeichert
            // Problem ist die Messagegrösse (max. 256 KB im Standard!)
            return moviemsg;
        }

        /*
        * Konvertiert WeatherForecastMsg in ein WeatherForecast-Objekt
        */
        public Film GetFilm(FilmMsg moviemsg)
        {
            Film movie = new Film();
            movie.Id = moviemsg.Id;
            movie.Name = moviemsg.Name;
            movie.Rating = moviemsg.Rating;
            movie.Feeling = moviemsg.Feeling;
            // Bild wird separat gespeichert
            // Problem ist die Messagegrösse (max. 256 KB im Standard!)
            return movie;
        }
    }
}
