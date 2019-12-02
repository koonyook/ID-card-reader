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
using System.ComponentModel;

namespace RealApplication01
{
	public class BusyBinding : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		private bool MyBusy = false;

		public bool myBusy
		{
			set
			{
				MyBusy = value;
				update();
			}
		}

		public bool isBusy
		{
			get
			{
				return myvar.domainContext.IsLoading 
					|| myvar.domainContext.IsSubmitting
					|| this.MyBusy;
			}
		}

		public string text
		{
			get
			{
				if (MyBusy == true)
					return "Processing...";
				if (myvar.domainContext.IsLoading == true)
					return "Loading...";
				if (myvar.domainContext.IsSubmitting == true)
					return "Submitting...";

				return "Please wait...";
			}
		}

		public void update()
		{
			OnPropertyChanged(new PropertyChangedEventArgs("isBusy"));
			OnPropertyChanged(new PropertyChangedEventArgs("text"));
		}
	}
}
