using UnityEditor;

namespace ND.UI.NDUI.TextEditor
{
    public static class LocalizationDebugModeMenu
    {
        private const string MenuName = "UICreator/Localization/Localization Debug Mode";
        private const string SettingName = "EditorPrefs_LocalizationDebugMode";
        
        public static bool IsEnabled
        {
            get { return EditorPrefs.GetBool(SettingName, false); }
            set { EditorPrefs.SetBool(SettingName, value); }
        }
        
        //[MenuItem(MenuName)]
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        }
      
        //[MenuItem(MenuName, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MenuName, IsEnabled);
            return true;
        }
    }
}