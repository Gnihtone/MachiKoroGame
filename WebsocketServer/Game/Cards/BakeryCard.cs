namespace WebsocketServer.Game.Cards
{
    public class BakeryCard : Card
    {
        public override string Name { get; set; } = "bakery";
        public override CardColor Color { get; set; } = CardColor.Green;
        public override CardType Type { get; set; } = CardType.Shop;
        public override int Cost { get; set; } = 1;
        public override int RollMin { get; set; } = 2;
        public override int RollMax { get; set; } = 3;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 1 + (player.UpgradedShops ? 1 : 0);
        }

        public override Card GetNewCard()
        {
            return new BakeryCard();
        }
    }
}
