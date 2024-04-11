namespace WebsocketServer.Game.Cards
{
    public class Cafe : Card
    {
        public override string Name { get; set; } = "cafe";
        public override CardColor Color { get; set; } = CardColor.Red;
        public override CardType Type { get; set; } = CardType.Cafe;
        public override int Cost { get; set; } = 2;
        public override int RollMin { get; set; } = 3;
        public override int RollMax { get; set; } = 3;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            int amount = Math.Min(board.CurrentPlayer.Money, 1 + (player.UpgradedShops ? 1 : 0));
            player.Money += amount;
            board.CurrentPlayer.Money -= amount;
        }

        public override Card GetNewCard()
        {
            return new Cafe();
        }
    }
}
