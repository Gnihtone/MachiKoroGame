namespace WebsocketServer.Game.Cards
{
    public class Stadium : Card
    {
        public override string Name { get; set; } = "stadium";
        public override CardColor Color { get; set; } = CardColor.Pink;
        public override CardType Type { get; set; } = CardType.Sight;
        public override int Cost { get; set; } = 6;
        public override int RollMin { get; set; } = 6;
        public override int RollMax { get; set; } = 6;
        public override bool NeedToChoose { get; set; } = false;

        public override void OnUse(GameBoard board, Player player, Player other)
        {
            foreach (Player player1 in board.players)
            {
                if (player1 == player) continue;

                int moneyGet = Math.Min(player1.Money, 2);
                player.Money += moneyGet;
                player1.Money -= moneyGet;
            }
        }

        public override Card GetNewCard()
        {
            return new Stadium();
        }
    }
}
