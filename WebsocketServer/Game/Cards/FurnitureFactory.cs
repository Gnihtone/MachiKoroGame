namespace WebsocketServer.Game.Cards
{
    public class FurnitureFactory : Card
    {
        public override string Name { get; set; } = "furniture";
        public override CardColor Color { get; set; } = CardColor.Green;
        public override CardType Type { get; set; } = CardType.Fabric;
        public override int Cost { get; set; } = 3;
        public override int RollMin { get; set; } = 8;
        public override int RollMax { get; set; } = 8;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 3 * player.GetAmountOfCards(CardType.Resources);
        }

        public override Card GetNewCard()
        {
            return new FurnitureFactory();
        }
    }
}
