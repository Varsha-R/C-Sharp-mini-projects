using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FetchUserProfiles
{
    class Program
    {
        static List<ManagementObject> UserAccountObjectResult = new List<ManagementObject>();
        static List<ManagementObject> UserProfileObjectResult = new List<ManagementObject>();
        static string CurrentLoggedinUserName = "";
        static string CurrentLoggedinUserDisplayName = "";
        static string CurrentLoggedinUserSID = "";

        private static string GetLoggedInUserName()
        {
            string userName = "";
            try
            {
                IEnumerable<ManagementObject> collection = WmiQueryExecutor.RunQuery("SELECT UserName FROM Win32_ComputerSystem");
                userName = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            }
            catch (Exception ex)
            {
                //logger.ErrorFormat(classType, "Failed to fetch Username from WMI{0}", ex);
                userName = Environment.UserName;
            }
            try
            {
                if (String.IsNullOrWhiteSpace(userName))
                {
                    //logger.InfoFormat(classType, "Failed to fetch Username from WMI. Falling back to Environment.UserName.");
                    userName = Environment.UserName;
                }
                //logger.Info(classType, "Fetching from Win32_UserAccount & Win32_UserProfile into userAccountObjectResult");
                UserAccountObjectResult = WmiQueryExecutor.RunQuery("Select * from Win32_UserAccount where LocalAccount=True AND Disabled=false")?.ToList();
                UserProfileObjectResult = WmiQueryExecutor.RunQuery("Select * from Win32_UserProfile")?.ToList();

                //logger.Info(classType, "Fetched  from Win32_UserAccount & Win32_UserProfile into userAccountObjectResult");
                CurrentLoggedinUserName = userName;
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                string currentUserSID = "";
                if (identity != null)
                {
                    currentUserSID = identity.User.ToString();
                }

                //logger.Info(classType, "Fetched current user sid from security identifier");

                //logger.Info(classType, "Fetching displayname from userAccountObjectResult");
                var accountResultsCollection = UserAccountObjectResult.AsEnumerable().Where((c) => c.Properties["SID"].Value.ToString() == currentUserSID);
                //logger.Info(classType, "Fetched displayname into accountResultsCollection");

                string displayName = userName;
                string name = "";
                if (accountResultsCollection.Any())
                {
                    name = (string)accountResultsCollection.Cast<ManagementBaseObject>().First()["Name"];
                    displayName = string.IsNullOrEmpty((string)accountResultsCollection.Cast<ManagementBaseObject>().First()["FullName"]) ? name : (string)accountResultsCollection.Cast<ManagementBaseObject>().First()["FullName"];
                    //logger.Debug(classType, "Display name fetched");
                }
                CurrentLoggedinUserDisplayName = displayName;
                //logger.Info(classType, "Setting current user sid to migration context");
                CurrentLoggedinUserSID = currentUserSID;
                //logger.Info(classType, "Setting current user sid to migration context completed");
            }
            catch (ArgumentNullException ane)
            {
                //logger.Error(classType, "Argument exception", ane);
            }
            catch (SystemException se)
            {
                //logger.Error(classType, "System exception", se);
            }

            Console.WriteLine("Username: {0}", userName);
            Console.WriteLine("Username: {0}", CurrentLoggedinUserDisplayName);
            Console.WriteLine("Username: {0}", CurrentLoggedinUserSID);
            return userName;
        }

        static void Main(string[] args)
        {
            string userName = GetLoggedInUserName();  
            string accountPicture = "";
            bool loggedInUser = false;
            List<string> usernames = new List<string>();
            usernames.Add(userName);
            string name;
            string sid;
            string displayName;
            List<Account> userProfiles = new List<Account>();

            foreach (ManagementBaseObject path in UserProfileObjectResult)
            {

                accountPicture = "";
                loggedInUser = false;
                string userPath = path["LocalPath"].ToString();
                if (userPath.Contains("Users\\"))
                {
                    //logger.Info(classType, "Adding user");
                    //parsing local path to fetch username
                    name = userPath.Split('\\').Last();
                    //logger.Debug(classType, "User Name fetched");
                    sid = path["SID"].ToString();
                    //logger.Debug(classType, "SID fetched");
                    //fetching displayname from Win32_UserAccount
                    //logger.Debug(classType, "Fetching displayname from userAccountObjectResult");
                    IEnumerable<ManagementObject> accountResultsCollection = UserAccountObjectResult.AsEnumerable().Where((c) => c.Properties["SID"].Value.ToString() == sid);
                    //logger.Debug(classType, "Fetched displayname into accountResultsCollection");
                    displayName = name;
                    //logger.Debug(classType, "User Home Location fetched");
                    //Fetching display name and user name
                    if (accountResultsCollection.Any())
                    {
                        name = (string)accountResultsCollection.Cast<ManagementBaseObject>().First()["Name"];
                        displayName = string.IsNullOrEmpty((string)accountResultsCollection.Cast<ManagementBaseObject>().First()["FullName"]) ? name : (string)accountResultsCollection.Cast<ManagementBaseObject>().First()["FullName"];
                        //logger.Debug(classType, "Display name fetched");
                    }

                    if (string.Equals(CurrentLoggedinUserSID, sid, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //logger.Info(classType, "Adding Current loggedin user");
                        loggedInUser = true;
                    }

                    userProfiles.Add(new Account
                    {
                        UserName = name,
                        LoggedInUser = loggedInUser,
                        DisplayName = displayName,
                        AccountPicture = accountPicture,
                    });
                    //logger.Info(classType, "Added user to list");
                    usernames.Add(name);
                }
            }

            Console.ReadKey();
        }
    }
}
