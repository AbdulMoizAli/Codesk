using CodeskLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeskWeb.Areas.WorkSpace.Models
{
    public class SessionViewModel
    {
        public IEnumerable<EditorSettingValues> Settings { get; set; }

        public IEnumerable<EditorTheme> Themes { get; set; }

        public IEnumerable<EditorSetting> UserSettings { get; set; }

        public IEnumerable<EditorSettingValues> GetCursorStyles()
        {
            return Settings
                .Where(x => string.Equals(x.Setting.SettingName, "cursor style", StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<EditorSettingValues> GetCursorBlinking()
        {
            return Settings
                .Where(x => string.Equals(x.Setting.SettingName, "cursor blinking", StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<EditorSettingValues> GetFontWeight()
        {
            return Settings
                .Where(x => string.Equals(x.Setting.SettingName, "font weight", StringComparison.OrdinalIgnoreCase));
        }

        public bool IsSelected(int settingId, string settingValue)
        {
            return UserSettings.First(x => x.SettingId == settingId).DefaultValue == settingValue;
        }

        public string GetSettingValue(int settingId)
        {
            return UserSettings.First(x => x.SettingId == settingId).DefaultValue;
        }

        public bool IsChecked(int settingId)
        {
            return UserSettings.First(x => x.SettingId == settingId).DefaultValue == "on";
        }
    }
}