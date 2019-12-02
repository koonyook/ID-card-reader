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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using System.IO;
using System.Collections.ObjectModel;
using RealApplication01.Web;
using RealApplication01.Views;

namespace RealApplication01
{
    public partial class Home : Page
    {
		CameraController camera;
		GroupSelectionWindow groupWindow;

        public Home()
        {
            InitializeComponent();
			camera = new CameraController(this.deviceComboBox, this.rectangle1);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			myvar.currentPage = this;
			myvar.getNewCompanyList();
			myvar.domainContext.RejectChanges(); //will changes value when user click to home

			//clear the old temp image
			WebClient oWebClient = new WebClient();
			//string hostname = Application.Current.Host.Source.AbsoluteUri.Replace(Application.Current.Host.Source.AbsolutePath, "");
			oWebClient.UploadStringAsync(new Uri(Util.getRootPath() + "/DeleteTempImageHandler.ashx?username=" + User.username), "");

			if (User.permission == User.viewerPermissionName)
			{
				visitorButton.IsEnabled = false;
				deliverButton.IsEnabled = false;
				tempButton.IsEnabled = false;
				tempGroupButton.IsEnabled = false;
				regisButton.IsEnabled = false;
				editButton.Content = "ดูกลุ่มผู้รับเหมา";
				checkoutButton.Content = "ดูผู้เข้า";
			}

			if (User.permission == User.guardPermissionName
				|| User.permission == User.viewerPermissionName)
			{
				viewDataButton.Content = "ดูข้อมูล";
			}

			
        }

		
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
			camera.load();
        }
		
        private void visitorButton_Click(object sender, RoutedEventArgs e)
        {	
			if (deviceComboBox.SelectedIndex > -1)
			{
				CheckInVisitorPage.isDeliver=false;
				camera.onCompleted += getResultAndNavigateToVisitorPage;
				camera.capture();
			}
			else
			{
				MessageBox.Show("Please select camera.");
			}
			
        }
		private void deliverButton_Click(object sender, RoutedEventArgs e)
		{
			if (deviceComboBox.SelectedIndex > -1)
			{
				CheckInVisitorPage.isDeliver = true;
				camera.onCompleted += getResultAndNavigateToVisitorPage;
				camera.capture();
			}
			else
			{
				MessageBox.Show("Please select camera.");
			}
		}

		public void getResultAndNavigateToVisitorPage(object sender, EventArgs e)
		{
			camera.onCompleted -= getResultAndNavigateToVisitorPage;
			myvar.xmlString = (string)sender;

			NavigationService.Navigate(new Uri("/VisitorPage", UriKind.Relative));
		}
		
		private void regisButton_Click(object sender, RoutedEventArgs e)
		{
			if(deviceComboBox.SelectedIndex > -1)
			{
				//this will be set to other value if it is group editing
				RegisContractorPage.editGroupID = -1;  //this is new group
				camera.onCompleted += getResultAndNavigateToContractorPage;
				camera.capture();
			}
			else
			{
				MessageBox.Show("Please select camera.");
			}
		}
		public void getResultAndNavigateToContractorPage(object sender, EventArgs e)
		{
			camera.onCompleted -= getResultAndNavigateToContractorPage;
			myvar.xmlString = (string)sender;

			NavigationService.Navigate(new Uri("/ContractorPage", UriKind.Relative));
		}

		private void editButton_Click(object sender, RoutedEventArgs e)
		{
			//must show groupWindow
			groupWindow = new GroupSelectionWindow();
			groupWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(groupWindow_Closing_AndGoToEdit);
			groupWindow.Show();
		}

		void groupWindow_Closing_AndGoToEdit(object sender, EventArgs e)
		{
			if (groupWindow.DialogResult == true)
			{
				RegisContractorPage.editGroupID=groupWindow.answerGroupID;
				NavigationService.Navigate(new Uri("/ContractorPage", UriKind.Relative));

			}
		}

		private void tempButton_Click(object sender, RoutedEventArgs e)
		{
			if(deviceComboBox.SelectedIndex > -1)
			{
				//this will be set to other value if it is group editing
				//this is tell module that get data from xml
				TradeTemporaryPage.selectedGroupID = -1;  
			
				camera.onCompleted += getResultAndNavigateToTemporaryPage;
				camera.capture();
			}
			else
			{
				MessageBox.Show("Please select camera.");
			}
		}
		public void getResultAndNavigateToTemporaryPage(object sender, EventArgs e)
		{
			camera.onCompleted -= getResultAndNavigateToContractorPage;
			myvar.xmlString = (string)sender;

			NavigationService.Navigate(new Uri("/TemporaryPage", UriKind.Relative));
		}

		private void tempGroupButton_Click(object sender, RoutedEventArgs e)
		{
			groupWindow = new GroupSelectionWindow();
			groupWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(groupWindow_Closing_AndGoToTemporary);
			groupWindow.Show();
		}

		void groupWindow_Closing_AndGoToTemporary(object sender, EventArgs e)
		{
			if (groupWindow.DialogResult == true)
			{
				TradeTemporaryPage.selectedGroupID = groupWindow.answerGroupID;
				NavigationService.Navigate(new Uri("/TemporaryPage", UriKind.Relative));

			}
		}

		private void checkoutButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/CheckOutPage", UriKind.Relative));
		}

		private void viewDataButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/DataPage", UriKind.Relative));
		}


		private void button1_Click(object sender, RoutedEventArgs e)
		{
			//for debuging purpose.
			//textBox1.Text = Util.getRootPath();
			/*
			textBox1.Text = Application.Current.Host.Source.AbsoluteUri + "\n";
			textBox1.Text += Application.Current.Host.Source.AbsolutePath + "\n";
			textBox1.Text += Application.Current.Host.Source.ToString() + "\n";
			textBox1.Text += Application.Current.Host.Source.LocalPath + "\n";
			textBox1.Text += Application.Current.Host.Source.Fragment + "\n";
			textBox1.Text += Application.Current.Host.Source.Host + "\n";
			textBox1.Text += Application.Current.Host.Source.OriginalString + "\n";
			*/
			//string hostname = Application.Current.Host.Source.AbsoluteUri.Replace(Application.Current.Host.Source.AbsolutePath, "");

			//(myvar.currentPage as Home).textBox1.Text = hostname;
		}

    }
}