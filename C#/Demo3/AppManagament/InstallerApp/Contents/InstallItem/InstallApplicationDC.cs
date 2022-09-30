using Common;
using Common.Language;
using Common.License;
using Common.NotifyProperty;
using System;
using System.Diagnostics;
using System.IO;


namespace InstallerApp.Contents.InstallItem
{
    public class InstallApplicationDC
    {
        public LanguageDC LanguageDC { get; }
        public string AppNameVersion { get; }
        public IReadOnlyProperty<string> Information { get; } = new Property<string>();
        public IReadOnlyProperty<int> CounterMaxValue { get; } = new Property<int>();
        public IReadOnlyProperty<int> CounterCurrentValue { get; } = new Property<int>();


        readonly ApplicationClient installerClient;
        readonly string appName;
        readonly string major;
        readonly string minor;
        readonly string build;
        readonly string[] fileNames;
        readonly string filesDestination;
        readonly bool isTool;


        public InstallApplicationDC(LanguageDC languageDC, ApplicationClient installerClient, string appName, string major, string minor, string build, bool isTool)
        {
            LanguageDC = languageDC;
            this.installerClient = installerClient;
            this.appName = appName;
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.isTool = isTool;

            if (!isTool)
                installerClient.SendTextArray(Constants.GetMainApplicationFileNames, appName, major, minor, build);
            else
                installerClient.SendTextArray(Constants.GetToolApplicationFileNames, appName, major, minor, build);

            fileNames = installerClient.ReceiveTextArray();

            AppNameVersion = string.Format("{0} V{1}.{2}.{3}", appName, major, minor, build);
            filesDestination = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), Constants.SmartSolutionsDebrecen, AppNameVersion);

            CounterMaxValue.ToSettable().Value = fileNames.Length;
            CounterCurrentValue.ToSettable().Value = 0;
        }

        public void Install()
        {
            Directory.CreateDirectory(filesDestination);

            for (int i = 0; i < fileNames.Length; i++)
            {
                Information.ToSettable().Value = LanguageDC.FileDownloadColon.Value + " " + fileNames[i];

                if (!isTool)
                    installerClient.SendTextArray(Constants.GetMainApplicationFile, appName, major, minor, build, fileNames[i]);
                else
                    installerClient.SendTextArray(Constants.GetToolApplicationFile, appName, major, minor, build, fileNames[i]);

                string b64 = installerClient.ReceiveText();
                byte[] binData = Convert.FromBase64String(b64);
                string filePath = Path.Combine(filesDestination, fileNames[i]);
                File.WriteAllBytes(filePath, binData);

                if (fileNames[i].Contains("exe"))
                {
                    Information.ToSettable().Value = LanguageDC.CreateShortcut.Value;
                    Utils.CreateShortcutOnDesktop(filePath, AppNameVersion);
                }

                CounterCurrentValue.ToSettable().Value = i + 1;
            }

            Information.ToSettable().Value = LanguageDC.Finished.Value;
        }
    }
}