using CodeskLibrary.Connections;
using CodeskLibrary.Models;
using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace CodeskLibrary.DataAccess
{
    public static class SessionFileManager
    {
        public static async Task<FileType> GetFileTypeExtension(string fileType)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.QuerySingleOrDefaultAsync<FileType>("spGetFileTypeExtension", new { fileType }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<SessionFile> GetSessionFile(string emailAddress, string sessionKey, int fileTypeId)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.QuerySingleOrDefaultAsync<SessionFile>("spGetSessionFile", new { emailAddress, sessionKey, fileTypeId }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<SessionFile> SaveSessionFile(string emailAddress, string sessionKey, string filePath, int fileTypeId)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.QuerySingleOrDefaultAsync<SessionFile>("spSaveSessionFile", new { emailAddress, sessionKey, filePath, fileTypeId }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task UpdateFileTitle(int fileId, string fileTitle)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spUpdateFileTitle", new { fileId, fileTitle }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}