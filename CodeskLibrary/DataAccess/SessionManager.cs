using Dapper;
using System.Data;
using CodeskLibrary.Connections;
using System.Threading.Tasks;
using CodeskLibrary.Models;
using System.Collections.Generic;
using System.Linq;

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

        public static async Task<int> SaveParticipant(string userName, string sessionKey)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<int>("spSaveParticipant", new { userName, sessionKey }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task EndSession(string emailAddress, string endDateTime, string sessionKey)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spEndSession", new { emailAddress, endDateTime, sessionKey }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<List<Session>> GetSessions(string emailAddress)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return (await db.QueryAsync<Session>("spGetSessions", new { emailAddress }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false)).ToList();
        }

        public static async Task DeleteSession(int sessionId)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spDeleteSession", new { sessionId }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}