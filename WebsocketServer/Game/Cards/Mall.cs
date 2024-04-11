namespace WebsocketServer.Game.Cards
{
    public class Mall : Card
    {
        public override string Name { get; set; } = "mall";
        public override CardColor Color { get; set; } = CardColor.Yellow;
        public override CardType Type { get; set; } = CardType.Sight;
        public override int Cost { get; set; } = 10;
        public override int RollMin { get; set; } = 0;
        public override int RollMax { get; set; } = 0;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            return;
        }

        public override void OnBuild(Player player)
        {
            player.UpgradedShops = true;
        }

        public override Card GetNewCard()
        {
            return new Mall();
        }
    }
}
