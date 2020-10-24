using Microsoft.EntityFrameworkCore;
using Tristan.Data.Models;

namespace Tristan.Data.DataAccess
{
    public class TristanContext : DbContext
    {
        public TristanContext(DbContextOptions options) : base(options) { }

        public DbSet<TblDoc> TblDoc { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region TblDoc Builder
            var tblDocBuilder = modelBuilder.Entity<TblDoc>();

            tblDocBuilder
                .HasKey(doc => doc.Id);
            
            tblDocBuilder
                .Property(doc => doc.Filename)
                .HasMaxLength(maxLength: 100)
                .IsRequired();
            
            tblDocBuilder
                .Property(doc => doc.Extension)
                .HasMaxLength(maxLength: 10)
                .IsRequired();
            
            tblDocBuilder
                .Property(doc => doc.Path)
                .HasMaxLength(maxLength: 2048)
                .IsRequired();

            tblDocBuilder
                .Property(doc => doc.DestinationDir)
                .HasMaxLength(maxLength: 2048);

            tblDocBuilder
                .Property(doc => doc.OccurredOn)
                .IsRequired();
            #endregion
        }
    }
}