using Dapper;
using System.Data;
using CodeskLibrary.Connections;
using System.Threading.Tasks;

namespace CodeskLibrary.DataAccess
{
    public static class SessionManager
    {
        public static async Task SaveSession(string emailAddress, string startDateTime, string sessionKey)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spSaveNewSession", new { emailAddress, startDateTime, sessionKey }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task SaveParticipant(string userName, string sessionKey)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spSaveParticipant", new { userName, sessionKey }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task EndSession(string emailAddress, string endDateTime, string sessionKey)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spEndSession", new { emailAddress, endDateTime, sessionKey }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}