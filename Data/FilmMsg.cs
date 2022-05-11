namespace swi2grupp1WebAPI.Data
{

    /*
     * Datenobjekt Wettervorhersage für den Message-Broker, d.h. ohne Bild
     */
    public class FilmMsg
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Rating { get; set; }

        public string? Feeling { get; set; }

    }
}
