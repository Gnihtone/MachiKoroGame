namespace WebsocketServer.Models
{
    public class PlayerModel
    {
        public string Id { get; set; }
        public Dictionary<string, int> Cards { get; set; }
        public int Money { get; set; }
        public bool HasMall { get; set; }
        public bool HasPark { get; set; }
        public bool HasRadio { get; set; }
        public bool HasStation { get; set; }
    }
}
