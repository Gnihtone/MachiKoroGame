using WebsocketServer.Game.Cards;

namespace WebsocketServer.Game
{
    public class GameBoard
    {
        private static List<Card> _allCards = new List<Card>()
        {
            new WheatFieldCard(),
            new BakeryCard(),
            new AppleOrchad(),
            new Cafe(),
            new CheeseFactory(),
            new Forest(),
            new FruitMarket(),
            new FurnitureFactory(),
            new Mine(),
            new Restaurant(),
            new Shop(),
            new Stadium(),
        };

        private static List<Card> _allSights = new List<Card>()
        {
            new AmusementPark(),
            new Mall(),
            new Station(),
            new Radio(),
        };

        private Random random = new Random();

        public bool IsStarted { get; set; } = false;
        public int OnlinePlayers { get; set; } = 0;
        public int MaxPlayers { get; set; } = 100;
        public List<Player> players = new List<Player>();
        public Player CurrentPlayer { get; set; }
        int currentPlayerIdx;
        int nextPlayerIdx;
        bool builtSomething;
        public MoveType CurrentMove { get; private set; }
        public int LastRoll { get; set; }

        public Dictionary<Card, int> availableCards = new Dictionary<Card, int>();
        public int CurrentRollNum = 1;

        private void UpdateMove()
        {
            currentPlayerIdx = nextPlayerIdx;
            nextPlayerIdx = (currentPlayerIdx + 1) % players.Count;
            CurrentPlayer = players[currentPlayerIdx];
            CurrentMove = MoveType.Income;
            CurrentRollNum = 1;
            builtSomething = false;
        }

        public void Start()
        {
            nextPlayerIdx = random.Next(players.Count);
            UpdateMove();

            foreach (Card card in _allCards)
            {
                availableCards.Add(card, 6);
            }

            foreach (Player player in players)
            {
                player.cards.Add(_allCards[0], 1);
                player.cards.Add(_allCards[1], 1);
            }
        }

        private Card GetCardByName(string name)
        {
            Card? card = _allCards.FirstOrDefault(x => x.Name == name, null);
            if (card == null)
            {
                card = _allSights.FirstOrDefault(x => x.Name == name, null);
            }
            return card;
        }

        public bool Build(string name)
        {
            if (builtSomething)
            {
                return false;
            }
            Card? card = GetCardByName(name);
            if (card == null)
            {
                return false;
            }
            if (CurrentPlayer.Money < card.Cost || availableCards.ContainsKey(card) && availableCards[card] == 0)
            { 
                return false;
            }
            CurrentPlayer.Money -= card.Cost;
            card.OnBuild(CurrentPlayer);
            if (availableCards.ContainsKey(card))
            {
                if (!CurrentPlayer.cards.TryAdd(card, 1))
                {
                    CurrentPlayer.cards[card]++;
                }
                availableCards[card]--;
            }

            builtSomething = true;

            return true;
        }

        public void RollDices(int amount)
        {
            if (amount == 1)
            {
                LastRoll = random.Next(6) + 1;
            }
            else if (amount == 2)
            {
                int roll1 = random.Next(6) + 1;
                int roll2 = random.Next(6) + 1;
                LastRoll = roll1 + roll2;
                if (roll1 == roll2 && CurrentPlayer.ContinueOnDuble)
                {
                    nextPlayerIdx = currentPlayerIdx;
                }
            }
        }

        public bool Continue()
        {
            bool OnIncome()
            {
                bool DoCards(CardColor color)
                {
                    foreach (Player player in players)
                    {
                        if ((color == CardColor.Green || color == CardColor.Pink) && player != CurrentPlayer)
                        {
                            continue;
                        }
                        foreach (var kv in player.cards)
                        {
                            if (kv.Key.Color == color && kv.Key.CheckRollIsOk(LastRoll)) {
                                for (int i = 0; i < kv.Value; ++i)
                                {
                                    kv.Key.OnUse(this, player);
                                }
                            }
                        }
                    }
                    return true;
                }

                if (CurrentRollNum == 1)
                {
                    return false;
                }

                DoCards(CardColor.Red);
                DoCards(CardColor.Blue);
                DoCards(CardColor.Green);
                DoCards(CardColor.Pink);

                CurrentMove = MoveType.Build;

                return true;
            }

            bool OnBuild()
            {
                if (!builtSomething)
                {
                    return false;
                }
                UpdateMove();

                return true;
            }

            if (CurrentMove == MoveType.Income)
            {
                return OnIncome();
            }
            else if (CurrentMove == MoveType.Build)
            {
                return OnBuild();
            }
            return false;
        }
    }
}
