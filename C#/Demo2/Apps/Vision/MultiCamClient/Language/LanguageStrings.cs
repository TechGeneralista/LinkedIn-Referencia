using System.ComponentModel;
using System.Threading;
using System.Windows;


namespace MultiCamApp.Language
{
    public static class LanguageStrings
    {
        public static string Snapshoot
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Pillanatkép";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Version
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Verzió";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string SerialNo
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Sorozatszám";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string IPAddress
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "IP cím";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Name
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Név";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Disconnect
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Leválasztás";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Connect
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Kapcsolódás";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Scan
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Felderítés";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Subnet
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Alhálózat";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Connection
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Kapcsolódás";

                        //case "en-GB":
                        //case "en-US":
                        //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string Camera
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Kamera";

                    //case "en-GB":
                    //case "en-US":
                    //    return "Camera";
                }

                return cultureName;
            }
        }

        public static string ImageOptimization
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Képoptimalizálás";
                }

                return cultureName;
            }
        }

        public static string Preview
        {
            get
            {
                switch (cultureName)
                {
                    case "hu-HU":
                        return "Előnézet";
                }

                return cultureName;
            }
        }

        static string cultureName
        {
            get
            {
                if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
                    return "hu-HU";

                return Thread.CurrentThread.CurrentUICulture.Name;
            }
        }
    }
}
