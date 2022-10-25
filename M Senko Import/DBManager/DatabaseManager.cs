using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace M_Senko_Import.DBManager
{
    internal class DatabaseManager
    {
        public static IConfiguration Configuration => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        public DataContext Prototype()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(string.Format(Configuration.GetConnectionString("DataContext")));
            return new DataContext(optionsBuilder.Options);
        }
    }
}
