namespace swi2grupp1WebAPI.Data
{
    /*Datenobjekt Befehl an Azure Function für Get und Delete */
    public class Command
    {
        // Read oder Delete
        public string Befehl { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}