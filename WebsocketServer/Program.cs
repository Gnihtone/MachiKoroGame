using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketServer
{
    internal class LobbyServer : WebSocketBehavior
    {
        public static Dictionary<string, List<LobbyServer>> lobbies = new Dictionary<string, List<LobbyServer>>();
        private string? lobby_id_;

        protected override void OnMessage(MessageEventArgs e)
        {
            lobbies.TryAdd("hellolobby", new List<LobbyServer>());
            lobbies["hellolobby"].Add(this);
            Console.WriteLine($"Got message: {e.Data}");
            var msg = "Oh, hello";
            Send(msg);
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Client connected to lobby");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            lobbies["hellolobby"].Remove(this);
            Console.WriteLine($"Closed with reason: {e.Reason}");
        }
    }

    internal class GameServer : WebSocketBehavior
    {

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
                    Console.WriteLine(LobbyServer.lobbies["hellolobby"].Count);
                }
            }
            wssv.Stop();
        }
    }
}
