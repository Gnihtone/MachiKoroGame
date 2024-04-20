namespace WebsocketServer.Models
{
    public class PlayerModel
    {
        public string Id { get; set; }
        public Dictionary<string, int> Cards { get; set; }
        public int Money { get; set; }
        public Sights Sights { get; set; }
    }
}
