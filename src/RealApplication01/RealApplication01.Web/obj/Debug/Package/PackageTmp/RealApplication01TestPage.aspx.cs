using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RealApplication01.Web
{
	public partial class InitialSetting : System.Web.UI.Page
	{
		public string getUser()
		{
			if (HttpContext.Current.User.Identity.Name != null && HttpContext.Current.User.Identity.Name.Length > 0)
				return HttpContext.Current.User.Identity.Name;
			else
				return "";
		}
		
		public string getPermission()	//from file /UserConfig/permission.txt
		{
			string[] tmp;
			string ans = "";	//default will be seted in User.cs (constructor)
			StreamReader re = File.OpenText(HttpContext.Current.Server.MapPath("~/UserConfig/permission.txt"));
			string input = null;
			if (HttpContext.Current.User.Identity.Name.Contains('\\'))
			{
				while ((input = re.ReadLine()) != null)
				{
					tmp = input.Trim().Split(':');
					
					if (tmp.Count() == 2 && tmp[0].ToLower() == HttpContext.Current.User.Identity.Name.Split('\\')[1].ToLower())
					{
						ans = tmp[1].ToLower();
						break;
					}
				}
			}
			re.Close();

			return ans;
		}
	}
}