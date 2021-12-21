using CodeskLibrary.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using CodeskLibrary.Connections;
using System.Threading.Tasks;

namespace CodeskLibrary.DataAccess
{
    public static class EditorManager
    {
        public static async Task<(IEnumerable<EditorSettingValues>, IEnumerable<EditorTheme>, IEnumerable<EditorSetting>)> GetEditorSettings(string emailAddress)
        {
            using IDbConnection db = DbConnection.GetConnection();

            using var reader = await db.QueryMultipleAsync("spGetEditorSettings", new { emailAddress }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            var editorSettings = reader.Read<EditorSettingValues, EditorSetting, EditorSettingValues>(
                (value, setting) =>
                {
                    value.Setting = setting;
                    return value;
                }, splitOn: "SettingId");

            var editorThemes = reader.Read<EditorTheme>();

            var userSettings = reader.Read<EditorSetting>();

            return (editorSettings, editorThemes, userSettings);
        }

        public static async Task SaveUserEditorSetting(string emailAddress, int settingId, string settingValue)
        {
            using IDbConnection db = DbConnection.GetConnection();

            await db.ExecuteAsync("spSaveUserEditorSetting", new { emailAddress, settingId, settingValue }, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    }
}