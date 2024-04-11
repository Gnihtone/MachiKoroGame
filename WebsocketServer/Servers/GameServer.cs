using WebsocketServer.Game.Cards;
using WebsocketServer.Models;
using WebsocketServer.Game;
using WebSocketSharp;

namespace WebsocketServer.Servers
{

    internal class GameServer : Common
    {
        private static Dictionary<string, GameBoard> game_boards_ = new Dictionary<string, GameBoard>();
        GameBoard? board;

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
            if (board.IsStarted)
            {
                return;
            }
            board.players.Add(new Player(userId!));
            board.OnlinePlayers++;
            if (board.OnlinePlayers == board.MaxPlayers)
            {
                SendChanges(lobbyId, "start", "");
                board.Start();
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
            }
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
                game_boards_[lobbyId].OnlinePlayers--;
            }
        }
    }
}
