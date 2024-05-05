using WebsocketServer.Game.Cards;
using WebsocketServer.Models;
using WebsocketServer.Game;
using WebSocketSharp;
using System.Text.Json;
using System.Text;

namespace WebsocketServer.Servers
{

    internal class GameServer : Common
    {
        private static Dictionary<string, GameBoard> game_boards_ = new Dictionary<string, GameBoard>();
        static Dictionary<string, HashSet<Common>> lobbies = new Dictionary<string, HashSet<Common>>();
        GameBoard? board;

        private void SendAll(string type, string msg)
        {
            SendChanges(lobbies, lobbyId, type, msg);
        }

        private string GetPlayerInfoMsg()
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
                players.Add(new PlayerModel()
                {
                    Id = player.Id,
                    Money = player.Money,
                    Cards = cards,
                    Sights = new Sights()
                    {
                        mall = player.UpgradedShops,
                        park = player.ContinueOnDuble,
                        radio = player.CanReroll,
                        station = player.CanRollTwo,
                    },
                });
            }

            BoardModel boardModel = new BoardModel()
            {
                CurrentPlayerId = board.CurrentPlayer.Id,
                CurrentMoveType = (int)board.CurrentMove,
                AvailableBuildings = buildings,
                Players = players,
            };
            return JsonSerializer.Serialize(boardModel);
        }

        private void UpdateLobbies()
        {
            foreach (GameServer cm in lobbies[lobbyId])
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{httpServer}/user/updatelobby");
                request.Content = new StringContent($"{{\"user_id\": \"{cm.userId}\"}}", Encoding.UTF8, "application/json");
                SendRequest(request);
            }
        }

        private void SendPlayersInfoAll()
        {
            SendChanges(lobbies, lobbyId, "maininfo", GetPlayerInfoMsg());
        }

        private void SendPlayersInfo()
        {
            SendMsg("maininfo", GetPlayerInfoMsg());
        }

        private void SendWrongMsg()
        {
            SendMsg("wrong");
        }

        private void SendWinMsg()
        {
            SendChanges(lobbies, lobbyId, "win", board.CurrentPlayer.Id);
            UpdateLobbies();
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
            board.OnlinePlayers++;
            if (board.IsStarted)
            {
                SendPlayersInfo();
                return;
            }
            board.players.Add(new Player(userId!));
            if (board.OnlinePlayers == board.MaxPlayers && !board.IsStarted)
            {
                SendChanges(lobbies, lobbyId, "start", "");
                board.Start();
                SendPlayersInfoAll();
            }
        }

        private void HandleRoll()
        {
            if (board.CurrentMove != MoveType.Income || board.CurrentRollNum != 1 && board.CurrentRollNum != 2)
            {
                SendWrongMsg();
                return;
            }
            if (board.CurrentRollNum == 2 && !board.CurrentPlayer.CanReroll)
            {
                SendWrongMsg();
                return;
            }
            if (lastMessage.message == "2" && !board.CurrentPlayer.CanRollTwo)
            {
                SendWrongMsg();
                return;
            }
            if (lastMessage.message == "1" || lastMessage.message == "2")
            {
                board.RollDices(int.Parse(lastMessage.message));
                board.CurrentRollNum++;
                SendAll("roll", $"{((board.LastRoll >> 3) == 0 ? "" : board.LastRoll >> 3)} {board.LastRoll & 7}");
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
            SendPlayersInfoAll();
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
            Player player = board.CurrentPlayer; 
            if (player.CanReroll && player.CanRollTwo && player.ContinueOnDuble && player.UpgradedShops)
            {
                SendWinMsg();
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
