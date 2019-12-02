using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace RealApplication01
{
    public class User
    {
		public const string adminPermissionName = "admin";
		public const string guardPermissionName = "guard";
		public const string viewerPermissionName = "viewer";
		public const string blockedPermissionName = "blocked";

		//must add checking method for authorization later with a generic handler
        //at the server, can use HttpContext.Current.User.Identity.Name easily
        //get requirement later

        public static string username;
        public static string computername;
		public static string permission;

        public static void setNewUser(IDictionary<string,string> InitParams)
        {
			string userID = InitParams["UserAccount"];
			if (userID != null && userID.Contains("\\"))
			{
				username = userID.Split('\\')[1];
				computername = userID.Split('\\')[0];
			}
			else
			{
				username = "UsernameError";
				computername = "ComputerNameError";
			}
			permission = InitParams["Permission"];
			if (permission != adminPermissionName && permission != guardPermissionName && permission != blockedPermissionName)
			{
				permission = viewerPermissionName;	//this is default for anonymous
			}
        }

        public static string getString()
        {
            return username + "\\" + computername +" ("+ permission +")";
        }
    }
}
