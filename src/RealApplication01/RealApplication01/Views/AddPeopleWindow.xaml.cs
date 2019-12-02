using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using System.IO;
using System.Windows.Navigation;

namespace RealApplication01.Views
{
	public partial class AddPeopleWindow : ChildWindow
	{
		CameraController camera;

		public AddPeopleWindow()
		{
			InitializeComponent();
			camera = new CameraController(this.deviceComboBox, this.rectangle1);
		}
		
		private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.LayoutRoot.DataContext = myvar.busyBinding;
			camera.load();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if(deviceComboBox.SelectedIndex > -1)
			{
				CheckInVisitorPage.isDeliver = false;
				camera.onCompleted += getResult;
				camera.capture();
			}
			else
			{
				MessageBox.Show("Please select camera.");
			}
		}

		public void getResult(object sender, EventArgs e)
		{
			camera.onCompleted -= getResult;
			myvar.xmlString = (string)sender;

			this.DialogResult = true;
			//NavigationService.Navigate(new Uri("/VisitorPage", UriKind.Relative));
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}

