using Common.Language;
using Common.NotifyProperty;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace Common.License
{
    public class LicenseDC
    {
        public event Action Exit;
        public LanguageDC LanguageDC { get; }
        public IReadOnlyProperty<string> TitleText { get; } = new Property<string>();
        public IProperty<string> LicenseKey { get; } = new Property<string>();


        readonly AppInfo appInfo;
        readonly Window mainWindow;
        readonly ApplicationClient applicationClient;
        readonly string licenseFileName = "license.txt";
        readonly string licenseFilePath;
        enum States { Activation, Trial, Normal }
        States status = States.Activation;


        public LicenseDC(LanguageDC languageDC, AppInfo appInfo, ApplicationClient applicationClient, Window mainWindow)
        {
            LanguageDC = languageDC;
            this.appInfo = appInfo;
            this.applicationClient = applicationClient;
            this.mainWindow = mainWindow;

            TitleText.ToSettable().Value = appInfo.ApplicationNameVersion;
            licenseFilePath = Path.Combine(appInfo.ApplicationDataPath, licenseFileName);
        }

        public Task CheckAsync(int intervalMillisec)
            => Task.Run(() => Check(intervalMillisec));

        private void Check(int intervalMillisec)
        {
            ReadLicenseKey();

            while (true)
            {
                switch (status)
                {
                    case States.Activation:
                        if (LicenseKey.Value == string.Empty)
                        {
                            Utils.InvokeIfNecessary(() =>
                            {
                                LicenseV licenseV = new LicenseV();
                                licenseV.Owner = mainWindow;
                                licenseV.DataContext = this;
                                licenseV.ShowDialog();
                            });

                            LicenseKey.Value = LicenseKey.Value.Trim();

                            if (LicenseKey.Value == string.Empty)
                                status = States.Trial;
                            else
                            {
                                if (!LicenseKey.Value.CheckLicenseKeyFormat())
                                {
                                    Utils.InvokeIfNecessary(() =>
                                    {
                                        MessageBox.Show(mainWindow, LanguageDC.KeyFormatIsIncorrect.Value, LanguageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
                                    });

                                    LicenseKey.Value = string.Empty;
                                }
                                else
                                    status = States.Normal;
                            }
                        }
                        else
                            status = States.Normal;

                        break;

                    case States.Trial:
                        if (applicationClient.Connect())
                        {
                            Exit?.Invoke();
                            return;
                        }

                        applicationClient.SendText(Constants.GetDemoRemainingDays);
                        int remainingdays = int.Parse(applicationClient.ReceiveText());
                        applicationClient.Disconnect();

                        if (remainingdays == 0)
                        {
                            Utils.InvokeIfNecessary(() =>
                            {
                                MessageBox.Show(mainWindow, LanguageDC.TheTrialVersionIsExpired.Value, LanguageDC.WarningColon.Value, MessageBoxButton.OK, MessageBoxImage.Warning);
                            });

                            Exit?.Invoke();
                            return;
                        }
                        else
                        {
                            TitleText.ToSettable().Value = string.Format("{0} - {1}: {2} {3}", appInfo.ApplicationNameVersion, LanguageDC.TrialVersion.Value, remainingdays, LanguageDC.DaysLeft.Value);
                            Thread.Sleep(intervalMillisec);
                        }

                        break;

                    case States.Normal:
                        if (applicationClient.Connect())
                        {
                            Exit?.Invoke();
                            return;
                        }

                        applicationClient.SendTextArray(Constants.CheckLicenseKey, LicenseKey.Value);
                        string[] result = applicationClient.ReceiveTextArray();
                        applicationClient.Disconnect();

                        if (result.Length == 1)
                        {
                            if (result[0] == Constants.IsNotValid)
                            {
                                Utils.InvokeIfNecessary(() =>
                                {
                                    MessageBox.Show(mainWindow, LanguageDC.TheKeyIsNotValid.Value, LanguageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
                                });

                                LicenseKey.Value = string.Empty;

                                if (File.Exists(licenseFilePath))
                                    File.Delete(licenseFilePath);

                                status = States.Activation;
                            }

                            else if (result[0] == Constants.SubscriptionIsExpired)
                            {
                                Utils.InvokeIfNecessary(() =>
                                {
                                    MessageBox.Show(mainWindow, LanguageDC.TheKeyIsExpired.Value, LanguageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
                                });

                                LicenseKey.Value = string.Empty;
                                status = States.Activation;
                            }

                            else if (result[0] == Constants.Eternal)
                            {
                                TitleText.ToSettable().Value = string.Format("{0} - {1} - {2}", appInfo.ApplicationNameVersion, LicenseKey.Value, LanguageDC.Eternal.Value);
                                WriteLicenseKey();
                                Thread.Sleep(intervalMillisec);
                            }

                            else if (result[0] == Constants.MultipleRuns)
                            {
                                Utils.InvokeIfNecessary(() =>
                                {
                                    MessageBox.Show(mainWindow, LanguageDC.MultipleInstanceRunError.Value, LanguageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
                                });

                                WriteLicenseKey();
                                Exit?.Invoke();
                                return;
                            }

                            else
                            {
                                Exit?.Invoke();
                                return;
                            }
                        }

                        else if (result.Length == 2)
                        {
                            if (result[0] == Constants.Subscription)
                            {
                                TitleText.ToSettable().Value = string.Format("{0} - {1} - {2}: {3} {4}", appInfo.ApplicationNameVersion, LicenseKey.Value, LanguageDC.SubscriptionIsActive.Value, result[1], LanguageDC.DaysLeft.Value);
                                WriteLicenseKey();
                                Thread.Sleep(intervalMillisec);
                            }

                            else
                            {
                                Exit?.Invoke();
                                return;
                            }
                        }

                        else
                        {
                            Exit?.Invoke();
                            return;
                        }

                        break;
                }
            }
        }

        public void ApplicationExit()
        {
            if (applicationClient.Connect())
            {
                Exit?.Invoke();
                return;
            }

            if(string.IsNullOrEmpty(LicenseKey.Value))
                applicationClient.SendText(Constants.ApplicationExit);
            else
                applicationClient.SendTextArray(Constants.ApplicationExit, LicenseKey.Value);

            applicationClient.Disconnect();
        }

        private void ReadLicenseKey()
        {
            if (File.Exists(licenseFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(licenseFilePath, Encoding.UTF8);

                    if (lines.Length == 1 && lines[0].CheckLicenseKeyFormat())
                    {
                        LicenseKey.Value = lines[0];
                        return;
                    }
                    else
                        File.Delete(licenseFilePath);
                }
                catch { }
            }

            LicenseKey.Value = string.Empty;
        }

        private void WriteLicenseKey()
        {
            if(!File.Exists(licenseFilePath))
            {
                try
                {
                    if (!Directory.Exists(appInfo.ApplicationDataPath))
                        Directory.CreateDirectory(appInfo.ApplicationDataPath);

                    File.WriteAllLines(licenseFilePath, new string[] { LicenseKey.Value }, Encoding.UTF8);
                }

                catch
                {

                }
            }
        }
    }
}
