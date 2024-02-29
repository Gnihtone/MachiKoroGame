namespace MachiKoroGame.Server.Models
{
    public struct Lobby
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<string> Players { get; set; }
    }
}
