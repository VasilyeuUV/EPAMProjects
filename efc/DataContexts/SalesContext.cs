using efc.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace efc.DataContexts
{
    public class SalesContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<FileName> Files { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        internal SalesContext(DbContextOptions<SalesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        internal SalesContext()
        {
            // при создании контекста автоматически проверит наличие базы данных и, если она отсуствует, создаст ее.
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-CSTF6K8;Initial Catalog=SalesDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //optionsBuilder.UseLoggerFactory(SalesLoggerFactory);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<FileName>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Manager>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Product>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }



        // устанавливаем фабрику логгера
        //public static readonly ILoggerFactory SalesLoggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name
        //                                        && level == LogLevel.Error)
        //           .AddProvider(new SalesLoggerProvider());    // указываем наш провайдер логгирования
        //});

    }
}
