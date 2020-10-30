using Microsoft.EntityFrameworkCore;
using Tristan.Core.Models;

namespace Tristan.Data.DataAccess
{
    public class TristanContext : DbContext
    {
        public TristanContext(DbContextOptions options) : base(options) { }

        public DbSet<Doc> Doc { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Doc Builder
            var DocBuilder = modelBuilder.Entity<Doc>();

            DocBuilder
                .HasKey(doc => doc.Id);
            
            DocBuilder
                .Property(doc => doc.Filename)
                .HasMaxLength(maxLength: 100)
                .IsRequired();
            
            DocBuilder
                .Property(doc => doc.Extension)
                .HasMaxLength(maxLength: 10)
                .IsRequired();
            
            DocBuilder
                .Property(doc => doc.Path)
                .HasMaxLength(maxLength: 2048)
                .IsRequired();

            DocBuilder
                .Property(doc => doc.DestinationDir)
                .HasMaxLength(maxLength: 2048);

            DocBuilder
                .Property(doc => doc.OccurredOn)
                .IsRequired();
            #endregion
        }
    }
}