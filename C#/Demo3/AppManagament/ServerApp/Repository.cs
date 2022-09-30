using Common.SQL;
using System.Collections.Generic;
using System.Data;


namespace ServerApp
{
    public class Repository
    {
        readonly SQLClient sqlClient;

        public Repository(string hostName, int port, string catalog, string user, string password)
        {
            sqlClient = new SQLClient(hostName, port, catalog, user, password);
        }

        public void Connect()
            => sqlClient.Connect();

        public void Disconnect()
            => sqlClient.Disconnect();

        internal string GetPrivacyPolicy(int languageId)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select ppLink
                    from PrivacyPolicy
                    where ppLanguageId = {0}
                ", languageId));

            return dataTable.Rows[0].Field<string>("ppLink");
        }

        internal string[] GetMainApps()
        {
            DataTable dataTable = sqlClient.ExecuteQuery(
               @"
                    select maName
                    from MainApplication
                ");

            List<string> mainAppNames = new List<string>();

            foreach (DataRow dataRow in dataTable.Rows)
                mainAppNames.Add(dataRow.Field<string>("maName"));

            return mainAppNames.ToArray();
        }

        internal string[] GetMainAppVersions(string mainAppName)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select mamvMajorVersion, mambvMinorVersion, mambvBuildVersion
                    from MainApplicationMinorBuildVersion
                    join MainApplicationMajorVersion on mamvId = mambvApplicationMajorVersionId
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                ", mainAppName));

            List<string> array = new List<string>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                array.Add(dataRow.Field<int>("mamvMajorVersion").ToString());
                array.Add(dataRow.Field<int>("mambvMinorVersion").ToString());
                array.Add(dataRow.Field<int>("mambvBuildVersion").ToString());
            }

            return array.ToArray();
        }

        internal string GetMainAppEULALink(string mainAppName, int languageId)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select eulaLink
                    from EndUserLicenseAgreement
                    join MainApplicationEndUserLicenseAgreementBind on eulaId = maeulabEndUserLicenseAgreementId
                    join MainApplication on maId = maeulabApplicationId
                    where maName = '{0}' and eulaLanguageId = {1}
                ", mainAppName, languageId));

            return dataTable.Rows[0].Field<string>("eulaLink");
        }

        internal string GetToolAppEULALink(string toolAppName, int languageId)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select eulaLink
                    from EndUserLicenseAgreement
                    join ToolApplicationEndUserLicenseAgreementBind on eulaId = taeulabEndUserLicenseAgreementId
                    join ToolApplication on taId = taeulabToolApplicationId
                    where taName = '{0}' and eulaLanguageId = {1}
                ", toolAppName, languageId));

            return dataTable.Rows[0].Field<string>("eulaLink");
        }

        internal int GetLicenseKeyDaysLeft(string maName, int majorVersion, string licenseKey)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    declare @lkExpireDateTime as datetime = (select lkExpireDateTime
										 from LicenseKey
										 join MainApplicationMajorVersion on mamvId = lkMainApplicationMajorVersionId and mamvMajorVersion = {1}
										 join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
										 where lkCode = '{2}')

                    declare @daysLeft as int

                    if @lkExpireDateTime is null
                    begin

	                    set @daysLeft = -1

                    end
                    else
                    begin

	                    set @daysLeft = datediff(day, getdate(), @lkExpireDateTime)

	                    if @daysLeft < 0
		                    set @daysLeft = 0

                    end

                    select @daysLeft [daysLeft]
                ", maName, majorVersion, licenseKey));

            return dataTable.Rows[0].Field<int>("daysLeft");
        }

        internal bool GetLicenseKeyRunRight(string maName, int majorVersion, string licenseKey, string hardwareId)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    declare @maName as varchar(50) = '{0}'
                    declare @mamvMajorVersion as int = {1}
                    declare @lkCode as varchar(100) = '{2}';
                    declare @personalComputerHardwareId as varchar(100) = '{3}'

                    declare @lkPersonalComputerHardwareId as varchar(100) = (select lkPersonalComputerHardwareId
														                     from LicenseKey
														                     join MainApplicationMajorVersion on mamvId = lkMainApplicationMajorVersionId and mamvMajorVersion = @mamvMajorVersion
														                     join MainApplication on maId = mamvMainApplicationId and maName = @maName
														                     where lkCode = @lkCode)

                    if @lkPersonalComputerHardwareId is null
                    begin

	                    update LicenseKey
	                    set lkPersonalComputerHardwareId = @personalComputerHardwareId
	                    from LicenseKey
	                    join MainApplicationMajorVersion on mamvId = lkMainApplicationMajorVersionId and mamvMajorVersion = @mamvMajorVersion
	                    join MainApplication on maId = mamvMainApplicationId and maName = @maName
	                    where lkCode = @lkCode

	                    select 1 [runRight]

                    end
                    else
                    begin

	                    if @lkPersonalComputerHardwareId = @personalComputerHardwareId
		                    select 1 [runRight]
	                    else
		                    select 0 [runRight]

                    end
                ", maName, majorVersion, licenseKey, hardwareId));

            return dataTable.Rows[0].Field<int>("runRight") == 1;
        }

        internal void LicenseKeyDeletePersonalComputerId(string maName, int majorVersion, string licenseKey, string hardwareId)
        {
            sqlClient.ExecuteNonQuery(string.Format(
               @"
                    update LicenseKey
                    set lkPersonalComputerHardwareId = null
                    from LicenseKey
                    join MainApplicationMajorVersion on mamvId = lkMainApplicationMajorVersionId and mamvMajorVersion = {1}
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                    where lkCode = '{2}' and lkPersonalComputerHardwareId = '{3}'
                ", maName, majorVersion, licenseKey, hardwareId));
        }

        internal bool GetLicenseKeyIsExist(string maName, int majorVersion, string licenseKey)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select count(*) [count]
                    from LicenseKey
                    join MainApplicationMajorVersion on mamvId = lkMainApplicationMajorVersionId and mamvMajorVersion = {1}
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                    where lkCode = '{2}'
                ", maName, majorVersion, licenseKey));

            return dataTable.Rows[0].Field<int>("count") == 1;
        }

        internal string[] GetToolApps(string anName, string major, string minor, string build)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select distinct taName
                    from MainToolApplicationBind
                    join MainApplicationMinorBuildVersion on mambvId = mtabMainApplicationMinorBuildVersionId and mambvMinorVersion = {2} and mambvBuildVersion = {3}
                    join MainApplicationMajorVersion on mamvId = mambvApplicationMajorVersionId and mamvMajorVersion = {1}
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                    join ToolApplicationVersion on tavId = mtabToolApplicationVersionId
                    join ToolApplication on taId = tavToolApplicationId
                ", anName, major, minor, build));

            List<string> array = new List<string>();

            foreach (DataRow dataRow in dataTable.Rows)
                array.Add(dataRow.Field<string>("taName").ToString());

            return array.ToArray();
        }

        internal string[] GetToolAppVersions(string maName, string mamvMajorVersion, string mambvMinorVersion, string mambvBuildVersion, string taName)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select tavMajorVersion, tavMinorVersion, tavBuildVersion
                    from MainToolApplicationBind
                    join MainApplicationMinorBuildVersion on mambvId = mtabMainApplicationMinorBuildVersionId and mambvMinorVersion = {2} and mambvBuildVersion = {3}
                    join MainApplicationMajorVersion on mamvId = mambvApplicationMajorVersionId and mamvMajorVersion = {1}
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                    join ToolApplicationVersion on tavId = mtabToolApplicationVersionId
                    join ToolApplication on taId = tavToolApplicationId and taName = '{4}'
                ", maName, mamvMajorVersion, mambvMinorVersion, mambvBuildVersion, taName));

            List<string> array = new List<string>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                array.Add(dataRow.Field<int>("tavMajorVersion").ToString());
                array.Add(dataRow.Field<int>("tavMinorVersion").ToString());
                array.Add(dataRow.Field<int>("tavBuildVersion").ToString());
            }

            return array.ToArray();
        }

        internal string[] GetMainApplicationFileNames(string maName, string mamvMajorVersion, string mambvMinorVersion, string mambvBuildVersion)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select fName
                    from [File]
                    join MainApplicationFileBind on mafbFileId = fId
                    join MainApplicationMinorBuildVersion on mambvApplicationMajorVersionId = mafbMainApplicationMinorBuildVersionId and mambvMinorVersion = {2} and mambvBuildVersion = {3}
                    join MainApplicationMajorVersion on mamvId = mambvApplicationMajorVersionId and mamvMajorVersion = {1}
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                ", maName, mamvMajorVersion, mambvMinorVersion, mambvBuildVersion));

            List<string> array = new List<string>();

            foreach (DataRow dataRow in dataTable.Rows)
                array.Add(dataRow.Field<string>("fName"));

            return array.ToArray();
        }

        internal string GetMainApplicationFile(string maName, string mamvMajorVersion, string mambvMinorVersion, string mambvBuildVersion, string fName)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select fBase64
                    from [File]
                    join MainApplicationFileBind on mafbFileId = fId
                    join MainApplicationMinorBuildVersion on mambvApplicationMajorVersionId = mafbMainApplicationMinorBuildVersionId and mambvMinorVersion = {2} and mambvBuildVersion = {3}
                    join MainApplicationMajorVersion on mamvId = mambvApplicationMajorVersionId and mamvMajorVersion = {1}
                    join MainApplication on maId = mamvMainApplicationId and maName = '{0}'
                    where fName = '{4}'
                ", maName, mamvMajorVersion, mambvMinorVersion, mambvBuildVersion, fName));

            return dataTable.Rows[0].Field<string>("fBase64");
        }

        internal string[] GetToolApplicationFileNames(string taName, string tavMajorVersion, string tavMinorVersion, string tavBuildVersion)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select fName
                    from [File]
                    join ToolApplicationFileBind on tafbFileId = fId
                    join ToolApplicationVersion on tafbToolApplicationVersionId = tavId and tavMajorVersion = {1} and tavMinorVersion = {2} and tavBuildVersion = {3}
                    join ToolApplication on taId = tavToolApplicationId and taName = '{0}'
                ", taName, tavMajorVersion, tavMinorVersion, tavBuildVersion));

            List<string> array = new List<string>();

            foreach (DataRow dataRow in dataTable.Rows)
                array.Add(dataRow.Field<string>("fName"));

            return array.ToArray();
        }

        internal string GetToolApplicationFile(string taName, string tavMajorVersion, string tavMinorVersion, string tavBuildVersion, string fName)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    select fBase64
                    from [File]
                    join ToolApplicationFileBind on tafbFileId = fId
                    join ToolApplicationVersion on tafbToolApplicationVersionId = tavId and tavMajorVersion = {1} and tavMinorVersion = {2} and tavBuildVersion = {3}
                    join ToolApplication on taId = tavToolApplicationId and taName = '{0}'
                    where fName = '{4}'
                ", taName, tavMajorVersion, tavMinorVersion, tavBuildVersion, fName));

            return dataTable.Rows[0].Field<string>("fBase64");
        }

        internal int GetDemoRemainingDays(string appName, int major, string hardwareId)
        {
            DataTable dataTable = sqlClient.ExecuteQuery(string.Format(
               @"
                    declare @maName as varchar(50) = '{0}'
                    declare @mamvMajorVersion as int = {1}
                    declare @dbPersonalComputerHardwareId as varchar(100) = '{2}'

                    declare @dbExpireDateTime as datetime = (select dbExpireDateTime
										                     from DemoBind
										                     join MainApplicationMajorVersion on mamvId = dbMainApplicationMajorVersionId and mamvMajorVersion = @mamvMajorVersion
										                     join MainApplication on maId = mamvMainApplicationId and maName = @maName
										                     where dbPersonalComputerHardwareId = @dbPersonalComputerHardwareId)

                    if @dbExpireDateTime is null
                    begin

	                    insert into DemoBind (dbPersonalComputerHardwareId, dbMainApplicationMajorVersionId, dbExpireDateTime)
	                    values
	                    (
		                    @dbPersonalComputerHardwareId,
		                    (select mamvId
		                     from MainApplicationMajorVersion
		                     join MainApplication on maId = mamvMainApplicationId and maName = @maName
		                     where mamvMajorVersion = @mamvMajorVersion),

		                    DATEADD(
				                    day, 
				                    (select mamvDemoVersionWillExpireDay
				                     from MainApplicationMajorVersion
				                     join MainApplication on maId = mamvMainApplicationId and maName = @maName
				                     where mamvMajorVersion = @mamvMajorVersion),
				                    getdate())
	                    )

	                    set @dbExpireDateTime = (select dbExpireDateTime
							                     from DemoBind
							                     join MainApplicationMajorVersion on mamvId = dbMainApplicationMajorVersionId and mamvMajorVersion = @mamvMajorVersion
							                     join MainApplication on maId = mamvMainApplicationId and maName = @maName
							                     where dbPersonalComputerHardwareId = @dbPersonalComputerHardwareId)

                    end

                    declare @daysLeft as int = datediff(day, getdate(), @dbExpireDateTime)

                    if @daysLeft < 0
                    begin
	                    set @daysLeft = 0
                    end

                    select @daysLeft [daysLeft]

                ", appName, major, hardwareId));

            return dataTable.Rows[0].Field<int>("daysLeft");
        }
    }
}
