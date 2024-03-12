using Microsoft.EntityFrameworkCore;
using MachiKoroGame.Server.DB.Context.Models;

namespace MachiKoroGame.Server.DB.Contexts
{
    public class CookieContext : DbContext
    {
        public DbSet<UserCookieId> UsersCookies { get; set; } = null!;
        public DbSet<UserInfo> UsersInfo { get; set; } = null!;
        public DbSet<Lobby> Lobbies { get; set; } = null!;

        public CookieContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=maindb;Username=postgres;Password=пароль_от_postgres");
        }
    }
}
