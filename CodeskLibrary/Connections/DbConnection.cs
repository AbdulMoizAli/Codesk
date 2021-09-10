using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

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