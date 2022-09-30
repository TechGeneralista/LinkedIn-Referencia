using Common;
using Common.Language;
using Common.License;
using System.Collections.Generic;
using System.Diagnostics;


namespace InstallerApp.Items
{
    public class MainApplicationItem
    {
        public LanguageDC LanguageDC { get; }
        public string MainAppName { get; }
        public MainApplicationVersionItem[] MainAppVersions { get; }


        readonly ApplicationClient installerClient;


        public MainApplicationItem(LanguageDC languageDC, ApplicationClient installerClient, string anName)
        {
            LanguageDC = languageDC;
            MainAppName = anName;
            this.installerClient = installerClient;

            installerClient.SendTextArray(Constants.GetMainAppVersions, anName);
            string[] mainAppVersions = installerClient.ReceiveTextArray();

            List<MainApplicationVersionItem> appVersions = new List<MainApplicationVersionItem>();
            int paramGroup = 3;
            int to = mainAppVersions.Length / paramGroup;
            for (int i = 0; i < to; i += paramGroup)
                appVersions.Add(new MainApplicationVersionItem(languageDC, installerClient, anName,
                    mainAppVersions[i], mainAppVersions[i+1], mainAppVersions[i+2]));

            MainAppVersions = appVersions.ToArray();
        }

        internal void ShowMainApplicationEndUserLicenseAgreementClick()
        {
            if (installerClient.Connect())
                return;

            installerClient.SendTextArray(Constants.GetMainAppEULALink, MainAppName, (LanguageDC.SelectedLanguageIndex.Value + 1).ToString());
            string privacyPolicyLink = installerClient.ReceiveText();
            installerClient.Disconnect();

            Process.Start(privacyPolicyLink);
        }
    }
}
