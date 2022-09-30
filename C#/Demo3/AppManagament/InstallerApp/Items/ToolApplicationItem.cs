using Common;
using Common.Language;
using Common.License;
using System.Collections.Generic;
using System.Diagnostics;


namespace InstallerApp.Items
{
    public class ToolApplicationItem
    {
        public LanguageDC LanguageDC { get; }
        public string ToolAppName { get; }
        public ToolApplicationVersionItem[] ToolAppVersions { get; }


        readonly ApplicationClient installerClient;


        public ToolApplicationItem(LanguageDC languageDC, ApplicationClient installerClient, string anName, string avMajor, string avMinor, string avBuild, string toolAppName)
        {
            LanguageDC = languageDC;
            ToolAppName = toolAppName;
            this.installerClient = installerClient;

            installerClient.SendTextArray(Constants.GetToolAppVersions, anName, avMajor, avMinor, avBuild, toolAppName);
            string[] parameters = installerClient.ReceiveTextArray();

            List<ToolApplicationVersionItem> toolAppVersions = new List<ToolApplicationVersionItem>();
            int paramGroup = 3;
            int to = parameters.Length / paramGroup;
            for (int i = 0; i < to; i += paramGroup)
                toolAppVersions.Add(new ToolApplicationVersionItem(parameters[i], parameters[i + 1], parameters[i + 2]));

            ToolAppVersions = toolAppVersions.ToArray();
        }

        internal void ShowToolApplicationEndUserLicenseAgreementClick()
        {
            if (installerClient.Connect())
                return;

            installerClient.SendTextArray(Constants.GetToolAppEULALink, ToolAppName, (LanguageDC.SelectedLanguageIndex.Value + 1).ToString());
            string privacyPolicyLink = installerClient.ReceiveText();
            installerClient.Disconnect();

            Process.Start(privacyPolicyLink);
        }
    }
}
