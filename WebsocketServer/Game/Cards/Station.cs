namespace WebsocketServer.Game.Cards
{
    public class Station : Card
    {
        public override string Name { get; set; } = "station";
        public override CardColor Color { get; set; } = CardColor.Yellow;
        public override CardType Type { get; set; } = CardType.Sight;
        public override int Cost { get; set; } = 4;
        public override int RollMin { get; set; } = 0;
        public override int RollMax { get; set; } = 0;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            return;
        }

        public override void OnBuild(Player player)
        {
            player.CanRollTwo = true;
        }

        public override Card GetNewCard()
        {
            return new Station();
        }   
    }
}
