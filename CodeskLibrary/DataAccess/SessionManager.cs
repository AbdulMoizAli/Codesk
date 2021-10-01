using CodeskLibrary.Connections;
using CodeskLibrary.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CodeskLibrary.DataAccess
{
    public static class SessionManager
    {
        public static async Task<(IEnumerable<EditorSettingValues>, IEnumerable<EditorTheme>)> GetEditorSettings()
        {
            using IDbConnection db = DbConnection.GetConnection();

            using var reader = await db.QueryMultipleAsync("spGetEditorSettings", commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            var editorSettings = reader.Read<EditorSettingValues, EditorSetting, EditorSettingValues>(
                (value, setting) =>
                {
                    value.Setting = setting;
                    return value;
                }, splitOn: "SettingId");

            var editorThemes = reader.Read<EditorTheme>();

            return (editorSettings, editorThemes);
        }
    }
}