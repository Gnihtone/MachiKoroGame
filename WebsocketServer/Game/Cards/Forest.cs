namespace WebsocketServer.Game.Cards
{
    public class Forest : Card
    {
        public override string Name { get; set; } = "forest";
        public override CardColor Color { get; set; } = CardColor.Blue;
        public override CardType Type { get; set; } = CardType.Resources;
        public override int Cost { get; set; } = 3;
        public override int RollMin { get; set; } = 5;
        public override int RollMax { get; set; } = 5;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            player.Money += 1;
        }

        public override Card GetNewCard()
        {
            return new Forest();
        }
    }
}
