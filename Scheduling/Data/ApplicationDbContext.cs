using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scheduling.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public IConfiguration Configuration { get; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            if (!optionsBuilder.IsConfigured)
            {
                if (DbManager.SiteName != null  && !optionsBuilder.IsConfigured)
                {
                    var dbName = DbManager.SiteName;
                    var dbConnectionString = DbManager.GetDbConnectionString(dbName, "Main");
                    optionsBuilder.UseSqlServer(dbConnectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
