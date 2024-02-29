using MachiKoroGame.Server.Models;

namespace MachiKoroGame.Server.DB
{
    public class PostgresDBController
    {
        private static PostgresDBController instance;
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

        public User GetUserByCookie(string id)
        {
            return new User { Id="4313" };
        }

        public Lobby CreateLobby(string password)
        {
            return new Lobby { };
        }

        public Lobby JoinLobby(string user_id, string password, string lobby_id)
        {
            return new Lobby { };
        }

        public bool LeaveLobby(string user_id) 
        {
            return true;
        }
    }
}
