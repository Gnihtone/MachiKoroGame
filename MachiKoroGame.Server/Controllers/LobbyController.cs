using Microsoft.AspNetCore.Mvc;
using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB;
using System.Text.Json;
using System.Web.Http.Cors;

namespace MachiKoroGame.Server.Controller
{
    internal class LobbyCreationInfo
    {
        public string name { get; set; }
        public int max_players { get; set; }
        public string? password { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        [HttpPost("Create")]
        public Lobby Create()
        {
            var str = new StreamReader(Request.Body).ReadToEnd();
            var data = JsonSerializer.Deserialize<LobbyCreationInfo>(str);
            return PostgresDBController.Singleton.CreateLobby(data.name, data.max_players, data.password);
        }

        [HttpGet("GetInfo")]
        public Lobby? GetLobby(string lobby_id)
        {
            return PostgresDBController.Singleton.GetLobbyById(lobby_id);
        }

        [HttpPost("Start")]
        public bool Start(string lobby_id)
        {
            return PostgresDBController.Singleton.StartLobby(lobby_id);
        }

        [HttpPost("Connect")]
        public bool Connect(string id)
        {
            return true;
        }
    }
}
