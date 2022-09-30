namespace Common
{
    public static class Constants
    {
        public static string SmartSolutionsDebrecen { get; } = "Smart Solutions Debrecen";
        public static string Ok { get; } = "Ok";
        public static string NotAvailable { get; } = "NotAvailable";
        public static byte[] HeartBeat { get; } = "63EA16486E8E49D4B8F845D565A22BE1".ToUTF8Bytes();
        public static string DataEnd { get; } = "<EOT>";
        public static byte[] Acknowledgment { get; } = "C48695C7A1DB4438B00C0704BBA35839".ToUTF8Bytes();
        public static char DivisionSign { get; } = '/';
        public static string GetPrivacyPolicyLink { get; } = "GetPrivacyPolicyLink";
        public static string GetMainApps { get; } = "GetMainApps";
        public static string GetToolApps { get; } = "GetToolApps";
        public static string GetMainAppVersions { get; } = "GetMainAppVersions";
        public static string GetToolAppVersions { get; } = "GetToolAppVersions";
        public static string GetMainAppEULALink { get; } = "GetMainAppEULALink";
        public static string GetToolAppEULALink { get; } = "GetToolAppEULALink";
        public static string AppLoginKey { get; } = "8C49DF2EB8D7431F9A2875D057ADDA02";
        public static string GetMainApplicationFileNames { get; } = "GetMainApplicationFileNames";
        public static string GetToolApplicationFileNames { get; } = "GetToolApplicationFileNames";
        public static string GetMainApplicationFile { get; } = "GetMainApplicationFile";
        public static string GetToolApplicationFile { get; } = "GetToolApplicationFile";
        public static string GetDemoRemainingDays { get; } = "GetDemoRemainingDays";
        public static string CheckLicenseKey { get; } = "CheckLicenseKey";
        public static string IsNotValid { get; } = "IsNotValid";
        public static string SubscriptionIsExpired { get; } = "SubscriptionIsExpired";
        public static string Subscription { get; } = "Subscription";
        public static string Eternal { get; } = "Eternal";
        public static string MultipleRuns { get; } = "MultipleRuns";
        public static string ApplicationExit { get; } = "D89D2CC2F89A49EB82CDB782B1A0F81B";
    }
}
