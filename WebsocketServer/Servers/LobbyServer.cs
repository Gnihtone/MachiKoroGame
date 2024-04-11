using System.Text.Json;
using System.Text;
using WebSocketSharp;

namespace WebsocketServer.Servers
{
    internal class LobbyServer : Common
    {
        private bool safe_exit = false;

        private void HandleExit()
        {
            if (lobbyId is not null)
            {
                lobbies[lobbyId].Remove(this);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"{httpServer}/user/updatelobby");
            request.Content = new StringContent($"{{\"user_id\": \"{userId}\"}}", Encoding.UTF8, "application/json");
            SendRequest(request);

            var lobby = GetLobbyInfo();
            if (lobby == null)
            {
                return;
            }
            string type = "lobby_update";
            string message = "[" + string.Join(", ", lobby.players) + "]";
            SendChanges(lobbyId, type, message);
            if (lobbyId == null)
            {
                return;
            }
        }

        private void HandleStart()
        {
            var lobby = GetLobbyInfo();
            if (lobby?.currentPlayers == 1)
            {
                var msg = new SocketServerMessage
                {
                    type = "bad"
                };
                Send(JsonSerializer.Serialize(msg));
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{httpServer}/lobby/start?lobby_id={lobbyId}");
                if (bool.Parse(SendRequest(request)))
                {
                    foreach (Common pl in lobbies[lobbyId])
                    {
                        if (pl == null)
                        {
                            continue;
                        }
                        if (pl is LobbyServer)
                        {
                            LobbyServer sr = pl as LobbyServer;
                            sr.safe_exit = true;
                        }
                    }
                    SendOkMsg();
                    SendChanges(lobbyId, "start", "");
                    safe_exit = true;
                }
                else
                {
                    SendBadMsg();
                }
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            if (lastMessage?.type == "exit")
            {
                HandleExit();
            }
            else if (lastMessage?.type == "start")
            {
                HandleStart();
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            if (!safe_exit)
            {
                HandleExit();
            }
        }
    }
}
