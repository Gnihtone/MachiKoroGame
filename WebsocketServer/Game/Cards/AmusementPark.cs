namespace WebsocketServer.Game.Cards
{
    public class AmusementPark : Card
    {
        public override string Name { get; set; } = "park";
        public override CardColor Color { get; set; } = CardColor.Yellow;
        public override CardType Type { get; set; } = CardType.Sight;
        public override int Cost { get; set; } = 16;
        public override int RollMin { get; set; } = 0;
        public override int RollMax { get; set; } = 0;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            return;
        }

        public override bool OnBuild(Player player)
        {
            if (player.ContinueOnDuble)
            {
                return false;
            }
            player.ContinueOnDuble = true;
            return true;
        }

        public override Card GetNewCard()
        {
            return new AmusementPark();
        }
    }
}
