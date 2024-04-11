namespace WebsocketServer.Game.Cards
{
    public class Shop : Card
    {
        public override string Name { get; set; } = "shop";
        public override CardColor Color { get; set; } = CardColor.Green;
        public override CardType Type { get; set; } = CardType.Shop;
        public override int Cost { get; set; } = 2;
        public override int RollMin { get; set; } = 4;
        public override int RollMax { get; set; } = 4;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 3 + (player.UpgradedShops ? 1 : 0);
        }

        public override Card GetNewCard()
        {
            return new Shop();
        }
    }
}
