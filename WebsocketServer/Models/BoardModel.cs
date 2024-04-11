namespace WebsocketServer.Models
{
    public class BoardModel
    {
        public Dictionary<string, int> AvailableBuildings { get; set; }
        public List<PlayerModel> Players { get; set; }
        public string CurrentPlayerId { get; set; }
        public int CurrentMoveType { get; set; }
    }
}
