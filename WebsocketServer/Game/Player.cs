using WebsocketServer.Game.Cards;

namespace WebsocketServer.Game
{
    public class Player
    {
        public string Id { get; private set; }
        public int Money { get; set; } = 3;
        public bool CanReroll { get; set; } = false;
        public bool CanRollTwo { get; set; } = false;
        public bool ContinueOnDuble { get; set; } = false;
        public bool UpgradedShops { get; set; } = false;

        public Dictionary<Card, int> cards = new Dictionary<Card, int>();

        public int GetAmountOfCards(CardType type)
        {
            int ans = 0;
            foreach (var kv in cards)
            {
                if (kv.Key.Type == type)
                {
                    ans += kv.Value;
                }
            }
            return ans;
        }

        public bool HasCard(CardType type)
        {
            return GetAmountOfCards(type) > 0;
        }

        public Player(string id)
        {
            Id = id;
        }
    }
}
