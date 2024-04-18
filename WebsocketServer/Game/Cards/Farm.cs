namespace WebsocketServer.Game.Cards
{
    public class Farm : Card
    {
        public override string Name { get; set; } = "farm";
        public override CardColor Color { get; set; } = CardColor.Blue;
        public override CardType Type { get; set; } = CardType.Farm;
        public override int Cost { get; set; } = 1;
        public override int RollMin { get; set; } = 2;
        public override int RollMax { get; set; } = 2;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 1;
        }

        public override Card GetNewCard()
        {
            return new BakeryCard();
        }
    }
}
