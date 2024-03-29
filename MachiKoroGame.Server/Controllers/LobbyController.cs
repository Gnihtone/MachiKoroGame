using Microsoft.AspNetCore.Mvc;
using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB;

namespace MachiKoroGame.Server.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        [HttpPost("Create")]
        public Lobby Create(string name, int max_players, string? password=null)
        {
            return PostgresDBController.Singleton.CreateLobby(name, max_players, password);
        }

        [HttpPost("Start")]
        public bool Start()
        {
            return true;
        }

        [HttpPost("Connect")]
        public bool Connect(string id)
        {
            return true;
        }
    }
}
