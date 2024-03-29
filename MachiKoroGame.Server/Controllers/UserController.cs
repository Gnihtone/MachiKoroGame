using Microsoft.AspNetCore.Mvc;
using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB;

namespace MachiKoroGame.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Get")]
        public User Get(string id)
        {
            var user = PostgresDBController.Singleton.GetUserByCookie(id);
            if (user == null)
            {
                return PostgresDBController.Singleton.CreateUser(id, id);
            }
            return user;
        }

        [HttpPost("UpdateLobby")]
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
