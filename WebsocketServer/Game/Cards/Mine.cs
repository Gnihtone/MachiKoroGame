namespace WebsocketServer.Game.Cards
{
    public class Mine : Card
    {
        public override string Name { get; set; } = "mine";
        public override CardColor Color { get; set; } = CardColor.Blue;
        public override CardType Type { get; set; } = CardType.Resources;
        public override int Cost { get; set; } = 6;
        public override int RollMin { get; set; } = 9;
        public override int RollMax { get; set; } = 9;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 5;
        }

        public override Card GetNewCard()
        {
            return new Mine();
        }
    }
}
