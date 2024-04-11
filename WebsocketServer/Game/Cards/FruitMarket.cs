namespace WebsocketServer.Game.Cards
{
    public class FruitMarket : Card
    {
        public override string Name { get; set; } = "market";
        public override CardColor Color { get; set; } = CardColor.Green;
        public override CardType Type { get; set; } = CardType.Fabric;
        public override int Cost { get; set; } = 2;
        public override int RollMin { get; set; } = 11;
        public override int RollMax { get; set; } = 12;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += player.GetAmountOfCards(CardType.Farm) * 2;
        }

        public override Card GetNewCard()
        {
            return new FruitMarket();
        }
    }
}
