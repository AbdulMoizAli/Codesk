namespace CodeskLibrary.Models
{
    public class EditorSettingValues
    {
        public int ValueId { get; set; }

        public string OptionValue { get; set; }

        public string OptionText { get; set; }

        public EditorSetting Setting { get; set; }
    }
}