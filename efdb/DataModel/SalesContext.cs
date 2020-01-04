namespace efdb.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class SalesContext : DbContext
    {
        public SalesContext()
            : base("name=SalesContext")
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<FileNameData> Files { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetForeignKeys(modelBuilder);
        }


        /// <summary>
        /// Set foreign keys
        /// </summary>
        /// <param name="modelBuilder"></param>
        private static void SetForeignKeys(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Sale>()
            //    // Setting relationships between tables
            //    .HasRequired(o => o.Manager)
            //    .WithMany(c => c.Sales)
            //    .HasForeignKey(o => o.ManagerId);       // Foreign Key Indication

            //modelBuilder.Entity<Sale>()
            //    // Setting relationships between tables
            //    .HasRequired(o => o.Product)
            //    .WithMany(c => c.Sales)
            //    .HasForeignKey(o => o.ProductId);       // Foreign Key Indication

            //modelBuilder.Entity<Sale>()
            //    // Setting relationships between tables
            //    .HasRequired(o => o.Client)
            //    .WithMany(c => c.Purchases)
            //    .HasForeignKey(o => o.Id);       // Foreign Key Indication

            //modelBuilder.Entity<Sale>()
            //    // Setting relationships between tables
            //    .HasRequired(o => o.FileName)
            //    .WithMany(c => c.Sales)
            //    .HasForeignKey(o => o.FileNameId);       // Foreign Key Indication
        }
    }
}