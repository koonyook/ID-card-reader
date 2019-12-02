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
	public class Renamer
	{
		public EventHandler onCompleted;

		public List<NamePair> namelist;
		public Renamer()
		{
			namelist = new List<NamePair>();
		}

		public void rename()
		{
			myvar.busyBinding.myBusy = true;
			string data="<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<renames>\n";
			
			foreach (NamePair pair in namelist)
			{
				data += String.Format("\t<rename oldName=\"{0}\" newName=\"{1}\"></rename>\n", pair.oldName, pair.newName);
			}
			data += "</renames>";

			WebClient oWebClient = new WebClient();

			//string hostname = Application.Current.Host.Source.AbsoluteUri.Replace(Application.Current.Host.Source.AbsolutePath, "");
			
			oWebClient.UploadStringCompleted += new UploadStringCompletedEventHandler(oWebClient_UploadStringCompleted);
			oWebClient.UploadStringAsync(new Uri(Util.getRootPath() + "/ImageRenameHandler.ashx?username=" + User.username), data);
		}

		private void oWebClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			myvar.busyBinding.myBusy = false;
			if(e.Result!=null)
				onCompleted(e.Result, null);
		}
	}

	public class NamePair
	{
		public string oldName;		//this shoud already have ".jpg"
		public string newName;		//this shoud already have ".jpg"
		public NamePair(string _oldName, string _newName)
		{
			oldName = _oldName;
			newName = _newName;
		}
	}
}
