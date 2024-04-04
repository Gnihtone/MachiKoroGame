namespace WebsocketServer.Models
{
    public class Lobby
    {
        public string id { get; set; }
        public string name { get; set; }
        public string? password { get; set; }
        public string[] players { get; set; }
        public int maxPlayers { get; set; }
        public int currentPlayers { get; set; }
        public bool isInGame { get; set; }
    }
}
