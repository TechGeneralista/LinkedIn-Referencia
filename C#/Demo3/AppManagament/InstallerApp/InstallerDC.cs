using Common;
using Common.Language;
using Common.License;
using Common.NotifyProperty;
using InstallerApp.Contents;
using System;
using System.Reflection;


namespace InstallerApp
{
    internal class InstallerDC
    {
        public event Action Finished;
        public LanguageDC LanguageDC { get; }
        public AppInfo AppInfo { get; }
        public ApplicationClient InstallerClient { get; }
        public IReadOnlyProperty<object> CurrentContent { get; } = new Property<object>();


        //readonly string hostName = "192.168.0.105";
        readonly string hostName = "185.75.194.85";
        readonly int serverPort = 8610;
        readonly string applicatonName = "Installer";
        readonly WelcomeDC welcomeDC;
        readonly AppSelectorDC appSelectorDC;
        readonly InstallDC installDC;


        public InstallerDC(LanguageDC languageDC, InstallerV installerV)
        {
            LanguageDC = languageDC;

            AppInfo = new AppInfo(Constants.SmartSolutionsDebrecen, applicatonName, Assembly.GetExecutingAssembly().GetName().Version);
            InstallerClient = new ApplicationClient(languageDC, AppInfo, installerV, hostName, serverPort, Utils.ConvertMegaBytesToBytes(1), Utils.ConvertMegaBytesToBytes(10), Utils.ConvertSecondsToMilliSeconds(30), Constants.DataEnd, Constants.DivisionSign.ToString());

            welcomeDC = new WelcomeDC(languageDC, InstallerClient);
            welcomeDC.ShowNextPage += ShowNextPage;

            appSelectorDC = new AppSelectorDC(languageDC, InstallerClient);
            appSelectorDC.ShowBackPage += ShowBackPage;
            appSelectorDC.ShowNextPage += ShowNextPage;

            installDC = new InstallDC(languageDC, InstallerClient, appSelectorDC);
            installDC.Finished += (o) => Finished?.Invoke();

            CurrentContent.ToSettable().Value = welcomeDC;
        }

        private void ShowNextPage(object obj)
        {
            if (obj == welcomeDC)
            {
                if (appSelectorDC.GetMainApps())
                    return;

                CurrentContent.ToSettable().Value = appSelectorDC;
            }

            else if (obj == appSelectorDC)
            {
                if (InstallerClient.Connect())
                    return;

                CurrentContent.ToSettable().Value = installDC;
                installDC.StartAsync();
            }
        }

        private void ShowBackPage(object obj)
        {
            if (obj == appSelectorDC)
                CurrentContent.ToSettable().Value = welcomeDC;
        }
    }
}