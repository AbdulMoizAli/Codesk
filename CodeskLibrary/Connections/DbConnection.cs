using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeskLibrary.Connections
{
    public static class DbConnection
    {
        public static SqlConnection GetConnection()
        {
            return new(GetConnectionString());
        }

        private static string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Development.json").Build();

            return config.GetConnectionString("Codeskdb");
        }
    }
}