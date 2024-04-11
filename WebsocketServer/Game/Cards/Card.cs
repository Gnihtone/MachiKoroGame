using System.Reflection.Metadata.Ecma335;

namespace WebsocketServer.Game.Cards
{
    public enum CardColor
    {
        Blue,
        Green,
        Yellow,
        Pink,
        Red
    }

    public enum CardType
    {
        Sight = 0,
        Farm = 1,
        Fabric = 2,
        Crops = 3,
        Cafe = 4,
        Resources = 5,
        Shop = 6
    }

    public enum MoveType
    {
        Income = 0,
        Build = 1
    }

    public abstract class Card
    {
        public abstract string Name { get; set; }
        public abstract CardColor Color { get; set; }
        public abstract CardType Type { get; set; }
        public abstract int Cost { get; set; }
        public abstract int RollMin { get; set; }
        public abstract int RollMax { get; set; }
        public abstract bool NeedToChoose { get; set; }

        public abstract void OnUse(GameBoard board, Player player, Player other = null);

        public virtual void OnBuild(Player player)
        {
            return;
        }

        public bool CheckRollIsOk(int roll)
        {
            return roll >= RollMin && roll <= RollMax;
        }

        public abstract Card GetNewCard();
    }
}
