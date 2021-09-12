using CodeskLibrary.Connections;
using CodeskLibrary.Models;
using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace CodeskLibrary.DataAccess
{
    public static class AccountManager
    {
        public static async Task<int> UserSignUp(User user)
        {
            var parameters = new
            {
                user.FirstName,
                user.LastName,
                user.UserName,
                user.EmailAddress,
                user.PasswordText
            };

            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<int>("spUserSignUp", parameters, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<User> UserSignIn(string emailAddress, string passwordText)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.QuerySingleOrDefaultAsync<User>("spUserSignIn", new { emailAddress, passwordText }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<bool> IsUniqueEmailAddress(string emailAddress)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<bool>("spValidateEmailAddress", new { emailAddress }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task<bool> IsUniqueUserName(string userName)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<bool>("spValidateUserName", new { userName }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}