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

namespace RealApplication01
{
	public class Util
	{
		public static string getRootPath()	//no slash at the end of path
		{
			string abs = Application.Current.Host.Source.AbsoluteUri;
			abs = abs.Remove(abs.LastIndexOf('/'));
			abs = abs.Remove(abs.LastIndexOf('/'));
			return abs;
		}
	}
}
