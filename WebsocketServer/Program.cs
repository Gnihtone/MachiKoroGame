using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using WebsocketServer.Servers;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketServer
{
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
