using efc.DataModels;
using Microsoft.EntityFrameworkCore;

namespace efc.DataContexts
{
    public class SalesContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<FileNameData> Files { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public SalesContext(DbContextOptions<SalesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


        //public SalesContext()
        //{
        //    // при создании контекста автоматически проверит наличие базы данных и, если она отсуствует, создаст ее.
        //    Database.EnsureCreated();
        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=DESKTOP-CSTF6K8;Initial Catalog=SalesDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<FileNameData>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Manager>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<Product>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }





    }
}
