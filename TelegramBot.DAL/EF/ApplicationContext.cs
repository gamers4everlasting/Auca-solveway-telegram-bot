using EntityFrameworkCore.CommonTools;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.EF
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public override int SaveChanges()
        {
            this.UpdateTrackableEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.UpdateTrackableEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }
    }
}