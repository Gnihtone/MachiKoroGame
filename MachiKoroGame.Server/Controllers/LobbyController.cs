using Microsoft.AspNetCore.Mvc;
using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB;

namespace MachiKoroGame.Server.Controllers
{
    [ApiController]
    [Route("user")]
    public class LobbyController : ControllerBase
    {
        [HttpPost(Name="CreateNewLobby")]
        public Lobby Create(string? password=null)
        {
            return PostgresDBController.Singleton.CreateLobby(password);
        }

        [HttpPost(Name="StartGame")]
        public bool Start()
        {
            return true;
        }

        [HttpPost(Name="ConnectToTheGame")]
        public bool Connect(string id)
        {
            return true;
        }
    }
}
