using Common;
using Common.Language;
using Common.License;
using InstallerApp.Items;
using System;
using System.Collections.Generic;
using System.Windows;


namespace InstallerApp.Contents
{
    public class AppSelectorDC
    {
        public event Action<object> ShowNextPage;
        public event Action<object> ShowBackPage;
        public LanguageDC LanguageDC { get; }
        public MainApplicationItem[] MainApps { get; private set; } = new MainApplicationItem[0];


        readonly ApplicationClient installerClient;


        public AppSelectorDC(LanguageDC languageDC, ApplicationClient installerClient)
        {
            LanguageDC = languageDC;
            this.installerClient = installerClient;
        }

        public bool GetMainApps()
        {
            if (MainApps.Length == 0)
            {
                if (installerClient.Connect())
                    return true;

                installerClient.SendText(Constants.GetMainApps);
                string[] mainAppNames = installerClient.ReceiveTextArray();

                List<MainApplicationItem> mainApps = new List<MainApplicationItem>();

                foreach (string mainAppName in mainAppNames)
                    mainApps.Add(new MainApplicationItem(LanguageDC, installerClient, mainAppName));

                MainApps = mainApps.ToArray();
                installerClient.Disconnect();
            }

            return false;
        }

        internal void BackButtonClick()
            => ShowBackPage?.Invoke(this);

        internal void NextButtonClick()
        {
            bool selected = false;

            foreach(MainApplicationItem mainApplicationItem in MainApps)
            {
                foreach(MainApplicationVersionItem mainApplicationVersionItem in mainApplicationItem.MainAppVersions)
                {
                    foreach(ToolApplicationItem toolApplicationItem in mainApplicationVersionItem.ToolApps)
                    {
                        foreach(ToolApplicationVersionItem toolApplicationVersionItem in toolApplicationItem.ToolAppVersions)
                        {
                            if(mainApplicationVersionItem.IsSelected.Value || toolApplicationVersionItem.IsSelected.Value)
                            {
                                selected = true;
                                break;
                            }
                        }
                    }
                }
            }

            if(selected)
                ShowNextPage?.Invoke(this);
            else
                MessageBox.Show(LanguageDC.PleaseSelectMainAndOrToolApplicationToContinue.Value, LanguageDC.WarningColon.Value, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
