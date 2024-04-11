namespace WebsocketServer.Game.Cards
{
    public class CheeseFactory : Card
    {
        public override string Name { get; set; } = "cheese";
        public override CardColor Color { get; set; } = CardColor.Green;
        public override CardType Type { get; set; } = CardType.Fabric;
        public override int Cost { get; set; } = 5;
        public override int RollMin { get; set; } = 7;
        public override int RollMax { get; set; } = 7;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += player.GetAmountOfCards(CardType.Farm) * 3;
        }

        public override Card GetNewCard()
        {
            return new CheeseFactory();
        }
    }
}
