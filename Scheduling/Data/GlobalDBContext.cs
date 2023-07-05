using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Scheduling.Data.GlobalEntities;

namespace Scheduling.Data
{
    public partial class GlobalDBContext : DbContext
    {
        public GlobalDBContext()
        {
        }

        public GlobalDBContext(DbContextOptions<GlobalDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectGatewayMapping> ProjectGatewayMapping { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=DefaultConnectionGlobal");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
