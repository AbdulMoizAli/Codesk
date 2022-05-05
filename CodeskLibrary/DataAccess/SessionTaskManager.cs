using CodeskLibrary.Connections;
using CodeskLibrary.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CodeskLibrary.DataAccess
{
    public static class SessionTaskManager
    {
        public static async Task<int> CreateTask(string emailAddress, string sessionKey, SessionTask task)
        {
            var parameters = new
            {
                emailAddress,
                sessionKey,
                task.TaskName,
                task.TaskDescription
            };

            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<int>("spCreateTask", parameters, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<SessionTask>> GetTasks(string sessionKey)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.QueryAsync<SessionTask>("spGetTasks", new { sessionKey }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<int> UpdateTask(string emailAddress, SessionTask task)
        {
            var parameters = new
            {
                emailAddress,
                task.TaskId,
                task.TaskName,
                task.TaskDescription
            };

            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<int>("spUpdateTask", parameters, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<int> DeleteTask(string emailAddress, int taskId)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<int>("spDeleteTask", new { emailAddress, taskId }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}