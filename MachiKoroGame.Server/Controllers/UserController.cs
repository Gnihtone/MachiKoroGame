using Microsoft.AspNetCore.Mvc;
using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB;
using System.Text.Json;
using System.Web.Http.Cors;

namespace MachiKoroGame.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Hello")]
        public string Hello()
        {
            return "Hello!";
        }

        [HttpGet("Get")]
        public User Get(string id)
        {
            var user = PostgresDBController.Singleton.GetUserByCookie(id);
            if (user == null)
            {
                return PostgresDBController.Singleton.CreateUser(id, Guid.NewGuid().ToString());
            }
            return user;
        }

        [HttpPost("UpdateLobby")]
        public Lobby? UpdateCurrentLobby()
        {
            var str = new StreamReader(Request.Body).ReadToEnd();
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(str);
            string user_id = data["user_id"];
            string? lobby_id = data.GetValueOrDefault("lobby_id", null);
            string? password = data.GetValueOrDefault("password", null);

            if (lobby_id == null)
            {
                PostgresDBController.Singleton.LeaveLobby(user_id);
                return new Lobby();
            }
            return PostgresDBController.Singleton.JoinLobby(user_id, password, lobby_id);
        }
    }
}
