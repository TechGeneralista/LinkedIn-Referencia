namespace Common.Settings
{
    public interface ICanSaveLoadSettings
    {
        void SaveSettings(ISettingsCollection settingsCollection);
        void LoadSettings(ISettingsCollection settingsCollection);
    }
}
