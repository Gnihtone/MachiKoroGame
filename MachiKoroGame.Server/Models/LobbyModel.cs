namespace MachiKoroGame.Server.Models
{
    public class Lobby
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        public string[]? Players { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public bool IsInGame { get; set; }
    }
}
