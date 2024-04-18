using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebsocketServer.Models;

namespace WebsocketServer.Servers
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

        protected static void SendChanges(Dictionary<string?, HashSet<Common>> lobbies, string? lobby_id, string type, string message, string? extra = null)
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
                if (cl != null)
                {
                    cl.Send(JsonSerializer.Serialize(msg));
                }
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
                }
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }
    }
}
