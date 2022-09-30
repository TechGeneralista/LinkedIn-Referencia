using Common.Language;
using Common.License;
using Common.NotifyProperty;
using InstallerApp.Contents.InstallItem;
using InstallerApp.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace InstallerApp.Contents
{
    public class InstallDC
    {
        public event Action<object> Finished;
        public LanguageDC LanguageDC { get; }
        public IReadOnlyPropertyArray<InstallApplicationDC> InstallItems { get; } = new PropertyArray<InstallApplicationDC>();
        public IReadOnlyProperty<bool> FinishButtonIsEnabled { get; } = new Property<bool>(false);


        readonly ApplicationClient installerClient;
        readonly AppSelectorDC appSelectorDC;


        public InstallDC(LanguageDC languageDC, ApplicationClient installerClient, AppSelectorDC appSelectorDC)
        {
            LanguageDC = languageDC;
            this.installerClient = installerClient;
            this.appSelectorDC = appSelectorDC;
        }

        public Task StartAsync()
            => Task.Run(() => InstallApps());

        private void InstallApps()
        {
            List<InstallApplicationDC> installItems = new List<InstallApplicationDC>();

            foreach (MainApplicationItem mai in appSelectorDC.MainApps)
            {
                foreach (MainApplicationVersionItem mavi in mai.MainAppVersions)
                {
                    if (mavi.IsSelected.Value)
                        installItems.Add(new InstallApplicationDC(LanguageDC, installerClient, mai.MainAppName, mavi.Major, mavi.Minor, mavi.Build, false));
                
                    foreach(ToolApplicationItem tai in mavi.ToolApps)
                    {
                        foreach(ToolApplicationVersionItem tavi in tai.ToolAppVersions)
                        {
                            if (tavi.IsSelected.Value)
                                installItems.Add(new InstallApplicationDC(LanguageDC, installerClient, tai.ToolAppName, tavi.Major, tavi.Minor, tavi.Build, true));
                        }
                    }
                }
            }

            InstallItems.ToSettable().AddRange(installItems.ToArray());
            InstallItems.ForEach(x => x.Install());
            FinishButtonIsEnabled.ToSettable().Value = true;
            installerClient.Disconnect();
        }

        internal void FinishedButtonClick()
            => Finished?.Invoke(this);
    }
}
