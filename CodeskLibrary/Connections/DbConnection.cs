using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace CodeskLibrary.Connections
{
    public static class DbConnection
    {
        public static IConfiguration Configuration { get; set; }

        public static SqlConnection GetConnection()
        {
            return new(Configuration.GetConnectionString("Codeskdb"));
        }
    }
}