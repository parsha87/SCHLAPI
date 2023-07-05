using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Scheduling.Data.TimestampEntities;

namespace Scheduling.Data
{
    public partial class TimestampDBContext : DbContext
    {
        public TimestampDBContext()
        {
        }

        public TimestampDBContext(DbContextOptions<TimestampDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CurrentStatusForSensors> CurrentStatusForSensors { get; set; }
        public virtual DbSet<CurrentStatusForValves> CurrentStatusForValves { get; set; }
        public virtual DbSet<MetadataTables> MetadataTables { get; set; }
        public virtual DbSet<ValveTimespanDetails> ValveTimespanDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (DbManager.SiteName != null && !optionsBuilder.IsConfigured)
                {
                    var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "TimeStamp");
                    optionsBuilder.UseSqlServer(dbConnectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrentStatusForSensors>(entity =>
            {
                entity.Property(e => e.SlotId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CurrentStatusForValves>(entity =>
            {
                entity.Property(e => e.SlotId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MetadataTables>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ValveTimespanDetails>(entity =>
            {
                entity.Property(e => e.IsSchAlert).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
