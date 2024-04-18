using WebsocketServer.Game.Cards;
using WebsocketServer.Models;
using WebsocketServer.Game;
using WebSocketSharp;
using System.Text.Json;

namespace WebsocketServer.Servers
{

    internal class GameServer : Common
    {
        private static Dictionary<string, GameBoard> game_boards_ = new Dictionary<string, GameBoard>();
        static Dictionary<string, HashSet<Common>> lobbies = new Dictionary<string, HashSet<Common>>();
        GameBoard? board;

        private void SendPlayerInfo()
        {
            Dictionary<string, int> buildings = new Dictionary<string, int>();
            foreach (var kv in board.availableCards)
            {
                buildings[kv.Key.Name] = kv.Value;
            }
            List<PlayerModel> players = new List<PlayerModel>();
            foreach (var player in board.players)
            {
                Dictionary<string, int> cards = new Dictionary<string, int>();
                foreach (var card in player.cards)
                {
                    cards.Add(card.Key.Name, card.Value);
                }
                players.Add(new PlayerModel() {
                    Id = player.Id,
                    Money = player.Money,
                    Cards = cards,
                    HasMall = player.UpgradedShops,
                    HasPark = player.ContinueOnDuble,
                    HasRadio = player.CanReroll,
                    HasStation = player.CanRollTwo,
                });
            }

            BoardModel boardModel = new BoardModel() { 
                CurrentPlayerId = board.CurrentPlayer.Id,
                CurrentMoveType = (int)board.CurrentMove,
                AvailableBuildings = buildings,
                Players = players,
            };
            string msg = JsonSerializer.Serialize(boardModel);

            SendChanges(lobbies, lobbyId, "maininfo", msg);
        }

        private void SendWrongMsg()
        {
            SendMsg("wrong");
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            Console.WriteLine("Game has been started.");
        }

        private bool CheckIfCreated()
        {
            if (!game_boards_.ContainsKey(lobbyId))
            {
                if (game_boards_.TryAdd(lobbyId, new GameBoard()))
                {
                    Lobby? lobby = GetLobbyInfo();
                    if (lobby == null)
                    {
                        return false;
                    }
                    game_boards_[lobbyId].MaxPlayers = lobby.currentPlayers;
                }
            }
            return true;
        }

        private void HandleHello()
        {
            lobbies.TryAdd(lobbyId, new HashSet<Common>());
            lobbies[lobbyId].Add(this);
            if (board.IsStarted)
            {
                return;
            }
            board.players.Add(new Player(userId!));
            board.OnlinePlayers++;
            if (board.OnlinePlayers == board.MaxPlayers)
            {
                SendChanges(lobbies, lobbyId, "start", "");
                board.Start();
                SendPlayerInfo();
            }
        }

        private void HandleRoll()
        {
            if (board.CurrentMove != MoveType.Income || board.CurrentRollNum != 1)
            {
                SendWrongMsg();
                return;
            }
            if (lastMessage.message == "1" || lastMessage.message == "2")
            {
                board.RollDices(int.Parse(lastMessage.message));
                board.CurrentRollNum = 2;
                SendMsg("roll", $"{board.LastRoll}");
            }
            else
            {
                SendWrongMsg();
            }
            return;
        }

        private void HandleReroll()
        {
            if (board.CurrentMove != MoveType.Income || board.CurrentRollNum != 2)
            {
                SendWrongMsg();
                return;
            }
            if (lastMessage.message == "1" || lastMessage.message == "2")
            {
                board.RollDices(int.Parse(lastMessage.message));
                board.CurrentRollNum = 3;
                SendMsg("reroll", $"{board.LastRoll}");
            }
            else
            {
                SendWrongMsg();
            }
            return;
        }

        private void HandleContinue()
        {
            if (!board.Continue())
            {
                SendWrongMsg();
                return;
            }
            SendPlayerInfo();
        }

        private bool HandleCommon()
        {
            if (board.CurrentPlayer.Id != userId)
            {
                SendWrongMsg();
                return false;
            }
            return true;
        }

        private void HandleBuild()
        {
            if (!board.Build(lastMessage.message))
            {
                SendWrongMsg();
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);

            if (lobbyId == null)
            {
                SendBadMsg();
                return;
            }

            if (!CheckIfCreated())
            {
                SendBadMsg();
                return;
            }

            if (board == null) {
                board = game_boards_[lobbyId];
            }
            if (lastMessage!.type == "hello")
            {
                HandleHello();
                return;
            }
            if (!HandleCommon())
            {
                return;
            }
            switch (lastMessage.type)
            {
                case "roll":
                    HandleRoll();
                    break;
                case "reroll":
                    HandleReroll(); 
                    break;
                case "build":
                    HandleBuild();
                    break;
                case "continue":
                    HandleContinue();
                    break;
                default:
                    SendWrongMsg();
                    break;
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            if (lobbyId != null)
            {
                lobbies[lobbyId].Remove(this);
                game_boards_[lobbyId].OnlinePlayers--;
            }
        }
    }
}
