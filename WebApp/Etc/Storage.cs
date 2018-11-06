namespace WebApp.Etc
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class Storage : DbContext
    {

        public Storage() : base(Config.getConfig<Storage>()) { }

        public DbSet<WebSite> WebSites { get; set; }
        public DbSet<Account> Accounts { get; set; }


        public static void Auto()
        {
            using (var context = new Storage())
                context.Database.EnsureCreated();
        }


        public class Config
        {
            public static string DbType = "Sqlite";
            public static string ConnStr = "Data Source=storage.db";
            public static DbContextOptions getConfig<DB>() where DB : DbContext, new()
            {
                var optionsBuilder = new DbContextOptionsBuilder<DB>();
                switch (DbType)
                {
                    case "Sqlite": optionsBuilder.UseSqlite(ConnStr); break;
                    case "Memory": optionsBuilder.UseInMemoryDatabase(ConnStr); break;
                    default: optionsBuilder.UseSqlite(ConnStr); break;
                }
                
                return optionsBuilder.Options;
            }
        }
    }
}