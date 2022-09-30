using Common;
using Common.Language;
using Common.Settings;


namespace UCVisionResultExplorerApp
{
    public class MainDC
    {
        public string Title { get; }
        public LanguageDC LanguageDC { get; }
        public ResultExplorerDC ResultExplorerDC { get; }


        public MainDC(ISettingsCollection settingsCollection, LanguageDC languageDC, AppInfo appInfo)
        {
            LanguageDC = languageDC;
            Title = appInfo.ApplicationNameVersion;

            ResultExplorerDC = new ResultExplorerDC(settingsCollection, languageDC);
        }
    }
}
