using MachiKoroGame.Server.Models;
using MachiKoroGame.Server.DB.Contexts;
using MachiKoroGame.Server.DB.Context.Models;
using Npgsql.TypeMapping;

namespace MachiKoroGame.Server.DB
{
    public class PostgresDBController
    {
        private static PostgresDBController? instance;
        public static PostgresDBController Singleton
        {
            get
            {
                if (instance == null)
                {
                    instance = new PostgresDBController();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        private PostgresDBController()
        {
            
        }

        public User CreateUser(string cookie_id, string id)
        {
            CookieContext db = new CookieContext();
            db.UsersCookies.Add(new Context.Models.UserCookieId { CookieId = cookie_id, Id = id });
            db.UsersInfo.Add(new Context.Models.UserInfo { CurrentLobbyId = null, Id = id });
            db.SaveChanges();
            return new User { CurrentLobbyId = null, Id = id };
        }

        public User? GetUserByCookie(string id)
        {
            CookieContext db = new CookieContext();
            var userId = db.UsersCookies.First(user => user.CookieId == id)?.Id;
            if (userId == null)
            {
                return null;
            }
            var user = db.UsersInfo.First(user => user.Id == userId);
            if (user == null)
            {
                return null;
            }
            return new User { CurrentLobbyId = user.CurrentLobbyId, Id = user.Id };
        }

        public Models.Lobby CreateLobby(string name, int max_players, string? password = null)
        {
            var lobby_id = Guid.NewGuid().ToString();
            CookieContext db = new CookieContext();
            db.Lobbies.Add(new Context.Models.Lobby
            {
                Id = lobby_id,
                Name = name,
                Password = password,
                Players = new string[4],
                MaxPlayers = max_players,
                CurrentPlayers = 0,
                IsInGame = false
            });
            db.SaveChanges();

            return new Models.Lobby 
            {
                Id = lobby_id, 
                Name = name, 
                Password = password, 
                Players = new string[max_players], 
                MaxPlayers = max_players, 
                CurrentPlayers = 0, 
                IsInGame = false 
            };
        }

        public Models.Lobby? JoinLobby(string user_id, string password, string lobby_id)
        {
            CookieContext db = new CookieContext();
            var lobby = db.Lobbies.First(lobby => lobby.Id == lobby_id);
            if (lobby == null || lobby.Password == password || lobby.CurrentPlayers == lobby.MaxPlayers || lobby.Players.Contains(user_id))
            {
                return null;
            }
            lobby.Players[lobby.CurrentPlayers] = user_id;
            db.Update(lobby);

            var user = db.UsersInfo.First(user => user.Id == user_id);
            user.CurrentLobbyId = lobby_id;
            db.UsersInfo.Update(user);

            db.SaveChanges();

            return new Models.Lobby 
            { 
                Id = lobby.Id,
                Name = lobby.Name,
                Password = password,
                MaxPlayers = lobby.MaxPlayers,
                CurrentPlayers = lobby.CurrentPlayers,  
                IsInGame = lobby.IsInGame,
                Players = lobby.Players
            };
        }

        public bool LeaveLobby(string user_id)
        {
            CookieContext db = new CookieContext();
            var lobby_id = db.UsersInfo.First(user => user.Id == user_id)?.CurrentLobbyId;
            if (lobby_id == null) { 
                return false; 
            }

            var lobby = db.Lobbies.First(lobby => lobby.Id == lobby_id);
            if (lobby == null || !lobby.Players.Contains(user_id))
            {
                return false;
            }

            for (int i = 0; i < lobby.CurrentPlayers; ++i)
            {
                if (lobby.Players[i] == user_id)
                {
                    lobby.Players[i] = lobby.Players[lobby.CurrentPlayers - 1];
                    lobby.CurrentPlayers--;
                    lobby.Players[lobby.CurrentPlayers] = "";
                    break;
                }
            }
            db.Lobbies.Update(lobby);
            db.SaveChanges();

            return true;
        }
    }
}
