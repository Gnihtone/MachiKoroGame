using Microsoft.AspNetCore.Mvc;
using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB;

namespace MachiKoroGame.Server.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        [HttpGet(Name="GetUserInfo")]
        public User Get(string id)
        {
            return PostgresDBController.Singleton.GetUserByCookie(id);
        }

        [HttpPost(Name="UpdateUserLobby")]
        public Lobby UpdateCurrentLobby(string user_id, string? lobby_id = null, string? password = null)
        {
            if (lobby_id == null)
            {
                PostgresDBController.Singleton.LeaveLobby(user_id);
                return new Lobby();
            }
            return PostgresDBController.Singleton.JoinLobby(user_id, password, lobby_id);
        }
    }
}
