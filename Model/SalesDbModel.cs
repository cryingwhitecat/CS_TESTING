namespace InternTest.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SalesDbModel : DbContext
    {
        public SalesDbModel()
            : base("name=SalesDbModel")
        {
        }

        public virtual DbSet<Sales> Sales { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sales>()
                .Property(e => e.SalesSum)
                .HasPrecision(10, 4);
        }
    }
}
