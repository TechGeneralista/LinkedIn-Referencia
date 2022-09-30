using Common;
using Common.Communication.TCP5;
using Common.Log;


namespace ServerApp
{
    internal class ServiceDC
    {
        public string Name { get; }
        public int Major { get; }
        public int Minor { get; }
        public int Build { get; }
        public string PersonalComputerId { get; }


        readonly TCPClientHandler tcpClientHandler;
        readonly Repository repository;
        readonly ConsoleLog consoleLog;


        public ServiceDC(TCPClientHandler tcpClientHandler, Repository repository, ConsoleLog consoleLog, string[] receivedArray)
        {
            this.tcpClientHandler = tcpClientHandler;
            this.repository = repository;
            this.consoleLog = consoleLog;

            Name = receivedArray[0];
            Major = int.Parse(receivedArray[1]);
            Minor = int.Parse(receivedArray[2]);
            Build = int.Parse(receivedArray[3]);
            PersonalComputerId = receivedArray[4];
        }

        public void Start()
        {
            while (true)
            {
                string[] request = tcpClientHandler.ReceiveTextArray();

                if (request.Length == 0)
                    break;

                if (request.Length == 1)
                {
                    if (request[0] == Constants.GetMainApps)
                    {
                        NewMessage("Fő alkalmazások neveinek kérése");
                        tcpClientHandler.SendTextArray(repository.GetMainApps());
                        continue;
                    }

                    if (request[0] == Constants.GetDemoRemainingDays)
                    {
                        int remainingDays = repository.GetDemoRemainingDays(Name, Major, PersonalComputerId);

                        if (remainingDays != 0)
                            NewMessage(string.Format("Próbaverzió ({0} nap van hátra)", remainingDays));
                        else
                            NewMessage("Próbaverzió lejárt");

                        tcpClientHandler.SendText(remainingDays.ToString());
                        continue;
                    }

                    if(request[0] == Constants.ApplicationExit)
                    {
                        NewMessage("Az alkalmazást bezárták");
                        continue;
                    }
                }

                else if (request.Length == 2)
                {
                    if (request[0] == Constants.GetPrivacyPolicyLink && int.TryParse(request[1], out int languageId))
                    {
                        NewMessage("Adatvédelmi tájékoztató link kérése");
                        tcpClientHandler.SendText(repository.GetPrivacyPolicy(languageId));
                        continue;
                    }

                    if (request[0] == Constants.GetMainAppVersions)
                    {
                        NewMessage("Fő alkalmazás verzió kérése");
                        tcpClientHandler.SendTextArray(repository.GetMainAppVersions(request[1]));
                        continue;
                    }

                    if (request[0] == Constants.CheckLicenseKey)
                    {
                        if (repository.GetLicenseKeyIsExist(Name, Major, request[1]))
                        {
                            if (repository.GetLicenseKeyRunRight(Name, Major, request[1], PersonalComputerId))
                            {
                                int daysLeft = repository.GetLicenseKeyDaysLeft(Name, Major, request[1]);

                                if(daysLeft == -1)
                                {
                                    NewMessage(string.Format("Örök: {0}", request[1]));
                                    tcpClientHandler.SendTextArray(Constants.Eternal);
                                    continue;
                                }

                                if (daysLeft == 0)
                                {
                                    NewMessage(string.Format("Előfizetés lejárt: {0}", request[1]));
                                    tcpClientHandler.SendTextArray(Constants.SubscriptionIsExpired);
                                    continue;
                                }

                                if(daysLeft > 0)
                                {
                                    NewMessage(string.Format("Előfizetés: {0} - {1} nap van hátra", request[1], daysLeft));
                                    tcpClientHandler.SendTextArray(Constants.Subscription, daysLeft.ToString());
                                    continue;
                                }
                            }
                            else
                            {
                                NewMessage(string.Format("Egyidejű többszörös futtatás: {0}", request[1]));
                                tcpClientHandler.SendTextArray(Constants.MultipleRuns);
                                continue;
                            }
                        }
                        else
                        {
                            NewMessage(string.Format("Érvénytelen kulcs: {0}", request[1]));
                            tcpClientHandler.SendTextArray(Constants.IsNotValid);
                            continue;
                        }
                    }

                    if (request[0] == Constants.ApplicationExit)
                    {
                        NewMessage("Az alkalmazást bezárták");
                        NewMessage(string.Format("Kijelentkezés: {0}", request[1]));
                        repository.LicenseKeyDeletePersonalComputerId(Name, Major, request[1], PersonalComputerId);
                        continue;
                    }
                }

                else if (request.Length == 3)
                {
                    if (int.TryParse(request[2], out int languageId))
                    {
                        if (request[0] == Constants.GetMainAppEULALink)
                        {
                            NewMessage("Fő alkalmazás licensz link kérése");
                            tcpClientHandler.SendText(repository.GetMainAppEULALink(request[1], languageId));
                            continue;
                        }

                        else if (request[0] == Constants.GetToolAppEULALink)
                        {
                            NewMessage("Segéd alkalmazás licensz link kérése");
                            tcpClientHandler.SendText(repository.GetToolAppEULALink(request[1], languageId));
                            continue;
                        }
                    }
                }

                else if (request.Length == 5)
                {
                    if (request[0] == Constants.GetToolApps)
                    {
                        NewMessage("Segéd alkalmazások neveinek kérése");
                        tcpClientHandler.SendTextArray(repository.GetToolApps(request[1], request[2], request[3], request[4]));
                        continue;
                    }

                    if (request[0] == Constants.GetMainApplicationFileNames)
                    {
                        NewMessage("Fő alkalmazás fájlok neveinek kérése");
                        tcpClientHandler.SendTextArray(repository.GetMainApplicationFileNames(request[1], request[2], request[3], request[4]));
                        continue;
                    }

                    if (request[0] == Constants.GetToolApplicationFileNames)
                    {
                        NewMessage("Segéd alkalmazás fájlok neveinek kérése");
                        tcpClientHandler.SendTextArray(repository.GetToolApplicationFileNames(request[1], request[2], request[3], request[4]));
                        continue;
                    }
                }

                else if (request.Length == 6)
                {
                    if (request[0] == Constants.GetToolAppVersions)
                    {
                        NewMessage("Segéd alkalmazás verziók kérése");
                        tcpClientHandler.SendTextArray(repository.GetToolAppVersions(request[1], request[2], request[3], request[4], request[5]));
                        continue;
                    }

                    if (request[0] == Constants.GetMainApplicationFile)
                    {
                        NewMessage("Fő alkalmazás fájl kérése");
                        tcpClientHandler.SendText(repository.GetMainApplicationFile(request[1], request[2], request[3], request[4], request[5]));
                        continue;
                    }

                    if (request[0] == Constants.GetToolApplicationFile)
                    {
                        NewMessage("Segéd alkalmazás fájl kérése");
                        tcpClientHandler.SendText(repository.GetToolApplicationFile(request[1], request[2], request[3], request[4], request[5]));
                        continue;
                    }
                }

                break;
            }
        }

        private void NewMessage(string message)
            => consoleLog.NewMessage(string.Format("{0}/{1} - {2} V{3}.{4}.{5}/{6} - {7}", tcpClientHandler.IP, tcpClientHandler.Port, Name, Major, Minor, Build, PersonalComputerId, message));
    }
}