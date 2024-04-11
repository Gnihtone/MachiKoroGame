namespace WebsocketServer.Game.Cards
{
    public class WheatFieldCard : Card
    {
        public override string Name { get; set; } = "field";
        public override CardColor Color { get; set; } = CardColor.Blue;
        public override CardType Type { get; set; } = CardType.Crops;
        public override int Cost { get; set; } = 1;
        public override int RollMin { get; set; } = 1;
        public override int RollMax { get; set; } = 1;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 1;
        }

        public override Card GetNewCard()
        {
            return new WheatFieldCard();
        }
    }
}
