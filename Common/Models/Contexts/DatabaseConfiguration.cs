using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamSearch.Common.Models.Contexts
{
    public class DatabaseConfiguration : DbMigrationsConfiguration<DatabaseContext>
    {
        public DatabaseConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
