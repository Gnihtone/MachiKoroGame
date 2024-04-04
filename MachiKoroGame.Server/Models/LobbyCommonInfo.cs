namespace MachiKoroGame.Server.Models
{
    public class LobbyCommonInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
    }
}
