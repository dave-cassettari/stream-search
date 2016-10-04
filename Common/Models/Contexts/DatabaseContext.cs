using StreamSearch.Common.Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Contexts
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
                .Replace("StreamSearch.Common.Models.Entities.", "")
                .Replace(".", "_");
        }

        #region Validation + Lazy Loading Hack

        private ConcurrentDictionary<Type, ICollection<LazyProperty>> lazyPropertiesByType = new ConcurrentDictionary<Type, ICollection<LazyProperty>>();

        private ICollection<LazyProperty> GetLazyProperties(Type entityType)
        {
            return lazyPropertiesByType.GetOrAdd(
                entityType,
                innerEntityType =>
                {
                    if (Configuration.LazyLoadingEnabled == false)
                    {
                        return new List<LazyProperty>();
                    }

                    return innerEntityType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(pi => pi.CanRead)
                        .Where(pi => !(pi.GetIndexParameters().Length > 0))
                        .Where(pi => pi.GetGetMethod().IsVirtual)
                        .Where(pi => pi.GetCustomAttributes().Any(attr => typeof(ValidationAttribute).IsAssignableFrom(attr.GetType())))
                        .Select(
                            pi =>
                            {
                                Type propertyType = pi.PropertyType;

                                if (HasGenericInterface(propertyType, typeof(ICollection<>)))
                                {
                                    return new LazyProperty(pi.Name, LazyType.Collection);
                                }

                                if (typeof(Entity).IsAssignableFrom(propertyType))
                                {
                                    return new LazyProperty(pi.Name, LazyType.Reference);
                                }

                                return new LazyProperty(pi.Name, LazyType.Property);
                            }
                        )
                        .ToList();
                }
            );
        }

        private class LazyProperty
        {
            public string Name { get; private set; }
            public LazyType Type { get; private set; }

            public LazyProperty(string name, LazyType type)
            {
                Name = name;
                Type = type;
            }
        }

        private enum LazyType
        {
            Property,
            Reference,
            Collection,
        }

        public static bool HasGenericInterface(Type input, Type genericType)
        {
            return input
                .GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);
        }

        public static bool Exists<T>(IEnumerable<T> source, Predicate<T> predicate)
        {
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
