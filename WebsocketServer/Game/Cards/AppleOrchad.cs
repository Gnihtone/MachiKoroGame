namespace WebsocketServer.Game.Cards
{
    public class AppleOrchad : Card
    {
        public override string Name { get; set; } = "apple";
        public override CardColor Color { get; set; } = CardColor.Blue;
        public override CardType Type { get; set; } = CardType.Crops;
        public override int Cost { get; set; } = 3;
        public override int RollMin { get; set; } = 10;
        public override int RollMax { get; set; } = 10;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 3;
        }

        public override Card GetNewCard()
        {
            return new AppleOrchad();
        }
    }
}
