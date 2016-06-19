using StreamSearch.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Models.Contexts
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext()
            : base("DefaultConnection")
        {
            //var initialiser = new DropCreateDatabaseAlways<DatabaseContext>();
            var initialiser = new CreateDatabaseIfNotExists<DatabaseContext>();
            //var initialiser = new DropCreateDatabaseIfModelChanges<DatabaseContext>();
            //var initialiser = new MigrateDatabaseToLatestVersion<DatabaseContext, DatabaseConfiguration>();

            System.Data.Entity.Database.SetInitializer<DatabaseContext>(initialiser);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityTypes = GetType().Assembly.GetTypes()
                .Where(t => typeof(Entity).IsAssignableFrom(t)
                         && t.IsClass
                         && !t.IsAbstract
                         && !t.IsGenericType);

            foreach (var entityType in entityTypes)
            {
                modelBuilder.RegisterEntityType(entityType);
            }

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Types()
                .Configure(c => c.ToTable(GetTableName(c.ClrType)));
        }

        private static string GetTableName(Type entityType)
        {
            return entityType
                .FullName
                .Replace("StreamSearch.Models.Entities.", "")
                .Replace(".", "_");
        }
    }
}
