using Common;
using Common.Language;
using Common.License;
using System;
using System.Diagnostics;


namespace InstallerApp.Contents
{
    public class WelcomeDC
    {
        public event Action<object> ShowNextPage;
        public LanguageDC LanguageDC { get; }


        readonly ApplicationClient installerClient;


        public WelcomeDC(LanguageDC languageDC, ApplicationClient installerClient)
        {
            LanguageDC = languageDC;
            this.installerClient = installerClient;
        }

        internal void NextButtonClick()
            => ShowNextPage?.Invoke(this);

        internal void ShowPrivacyPolicyClick()
        {
            if (installerClient.Connect())
                return;

            installerClient.SendTextArray(Constants.GetPrivacyPolicyLink, (LanguageDC.SelectedLanguageIndex.Value + 1).ToString());
            string privacyPolicyLink = installerClient.ReceiveText();
            installerClient.Disconnect();

            Process.Start(privacyPolicyLink);
        }
    }
}
