using CodeskLibrary.Connections;
using CodeskLibrary.Models;
using Dapper;
using System;
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
                user.EmailAddress,
                user.PasswordText
            };

            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<int>("spUserSignUp", parameters, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task UserExternalSignUp(string emailAddress)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spUserExternalSignUp", new { emailAddress }, commandType: CommandType.StoredProcedure)
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

        public static async Task<Guid?> GetForgotPasswordToken(string emailAddress)
        {
            using IDbConnection db = DbConnection.GetConnection();

            return await db.ExecuteScalarAsync<Guid?>("spGetForgotPasswordToken", new { emailAddress }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }

        public static async Task ResetPassword(string passwordText, string token)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spResetPassword", new { passwordText, token }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}