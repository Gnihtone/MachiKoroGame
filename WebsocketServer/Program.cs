using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using WebsocketServer.Models;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketServer
{
    internal class SocketClientMessage
    {
        public string id { get; set; }
        public string type { get; set; }
        public string message { get; set; }
    }

    internal class SocketServerMessage
    {
        public string type { get; set; }
        public string message { get; set; }
        public string extra { get; set; }
    }

    internal class Common : WebSocketBehavior
    {
        protected static readonly HttpClient client = new HttpClient();

        protected static Dictionary<string, HashSet<Common>> lobbies = new Dictionary<string, HashSet<Common>>();
        
        protected string? lobbyId;
        protected string? userId;
        protected SocketClientMessage? lastMessage;

        protected bool isLeader = false;

        public static string httpServer = "http://localhost:5044/api";

        protected void SendMsg(string type, string? message = null, string? extra = null)
        {
            var msg = new SocketServerMessage
            {
                type = type,
                message = message,
                extra = extra
            };
            Send(JsonSerializer.Serialize(msg));
        }

        protected static void SendChanges(string? lobby_id, string type, string message, string? extra=null)
        {
            if (!lobbies.ContainsKey(lobby_id))
            {
                return;
            }
            var msg = new SocketServerMessage
            {
                type = type,
                message = message,
                extra = extra
            };
            foreach (Common cl in lobbies[lobby_id])
            {
                cl.Send(JsonSerializer.Serialize(msg));
            }
        }

        protected void SendBadMsg()
        {
            SendMsg("bad");
        }

        protected void SendOkMsg()
        {
            SendMsg("ok");
        }

        protected string? SendRequest(HttpRequestMessage request)
        {
            var response = client.Send(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var str = new StreamReader(response.Content.ReadAsStream());
            var str1 = str.ReadToEnd();
            return str1;
        }

        protected Lobby? GetLobbyInfo()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{httpServer}/lobby/getinfo?lobby_id={lobbyId}");
            var response = SendRequest(request);
            if (response == null || response == "")
            {
                return null;
            }
            var lobby = JsonSerializer.Deserialize<Lobby>(response);
            return lobby;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Client connected to lobby");
            SendMsg("hello");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            lastMessage = JsonSerializer.Deserialize<SocketClientMessage>(e.Data);
            Console.WriteLine(lastMessage);
            if (lastMessage?.type == "hello")
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{httpServer}/user/get?id={lastMessage.id}");
                var resp = SendRequest(request);
                if (resp == null)
                {
                    SendBadMsg();
                    return;
                }
                var user = JsonSerializer.Deserialize<User>(resp);

                userId = user?.id;
                lobbyId = user?.currentLobbyId;

                SocketServerMessage msg;
                if (lobbyId == null)
                {
                    SendBadMsg();
                } 
                else
                {
                    SendOkMsg();
                    lobbies.TryAdd(lobbyId, new HashSet<Common>());
                    lobbies[lobbyId].Add(this);

                    var lobby = GetLobbyInfo();
                    if (lobby == null)
                    {
                        return;
                    }
                    string type = "lobby_update";
                    string message = "[" + string.Join(", ", lobby.players) + "]";
                    SendChanges(lobbyId, type, message);
                }
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            if (lobbyId is not null)
            {
                lobbies[lobbyId].Remove(this);
            }
        }
    }

    internal class LobbyServer : Common
    {
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
                    SendOkMsg();
                    SendChanges(lobbyId, "start", "");
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
            HandleExit();
        }
    }

    internal class GameServer : Common
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var wssv = new WebSocketServer("ws://127.0.0.1");

            wssv.AddWebSocketService<LobbyServer>("/Lobby");
            wssv.AddWebSocketService<GameServer>("/Game");
            wssv.Start();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'X')
                {
                    break;
                }
                else if (key.KeyChar == 'A')
                {
                }
            }
            wssv.Stop();
        }
    }
}
