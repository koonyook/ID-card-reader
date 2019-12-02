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
using System.Windows.Navigation;
using System.ServiceModel.DomainServices.Client;
using RealApplication01.Web;

namespace RealApplication01.Views
{
	public partial class CheckOutPage : Page
	{
		private LoadOperation<Coming> lo1;
		private LoadOperation<Identifier> lo2;
		private LoadOperation<IdentifierImage> lo3;

		public CheckOutPage()
		{
			InitializeComponent();
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (User.permission == User.viewerPermissionName)
			{
				kickButton.IsEnabled = false;
				leaveAllButton.IsEnabled = false;
				leaveOneButton.IsEnabled = false;
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			lo1 = myvar.domainContext.Load(myvar.domainContext.GetComingsForCheckOutWithOutsiderQuery());
			lo1.Completed += waitForComplete;
			lo2 = myvar.domainContext.Load(myvar.domainContext.GetIdentifiersQuery());
			lo2.Completed += waitForComplete;
			lo3 = myvar.domainContext.Load(myvar.domainContext.GetIdentifierImagesQuery());
			lo3.Completed += waitForComplete;

			myvar.busyBinding.update();
		}

		public void waitForComplete(object sender, EventArgs e)
		{
			if (lo1.IsComplete
				&& lo2.IsComplete
				&& lo3.IsComplete)
			{
				myvar.busyBinding.update();
				loadEverything_Completed();
			}
			else
			{
				debug.Text += ">";
			}
		}

		public void loadEverything_Completed()
		{
			//load data to myvar.regisContractorPage
			myvar.everyCheckOutPage = new EveryCheckOutPage(this);

			this.LayoutRoot.DataContext = myvar.everyCheckOutPage;

			myvar.busyBinding.update();
		}

		private void searchButton_Click(object sender, RoutedEventArgs e)
		{
			//do nothing
			//searchString textBox is out of focus now.
			//then, searchString will be set and do screening
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(
				myvar.everyCheckOutPage.checkOutList[0].imageSource.UriSource.ToString());
		}

		private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				searchButton.Focus();
			}
		}

		private void sameTimeButton_Click(object sender, RoutedEventArgs e)
		{
			if(myvar.everyCheckOutPage.selectedCheckOut!=null)
			{
				myvar.everyCheckOutPage.enableFilter = false;
				myvar.everyCheckOutPage.selectedFromDate = myvar.everyCheckOutPage.selectedCheckOut.coming.TimeIn.Date;
				myvar.everyCheckOutPage.selectedToDate = myvar.everyCheckOutPage.selectedCheckOut.coming.TimeIn.Date;
				myvar.everyCheckOutPage.enableFilter = true;
				myvar.everyCheckOutPage.searchString = myvar.everyCheckOutPage.selectedCheckOut.checkInTime;
			}
			else
			{
				myvar.everyCheckOutPage.debug = "Please select";
			}
		}

		private void kickButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.everyCheckOutPage.debug = "";
			if (myvar.everyCheckOutPage.selectedCheckOut == null || myvar.everyCheckOutPage.selectedCheckOut.isVisible == false)
			{
				myvar.everyCheckOutPage.debug = "Please select.";
			}
			else
			{
				myvar.everyCheckOutPage.selectedCheckOut.submitLeave(true);
			}
		}

		private void leaveOneButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.everyCheckOutPage.debug = "";
			if (myvar.everyCheckOutPage.selectedCheckOut==null || myvar.everyCheckOutPage.selectedCheckOut.isVisible == false)
			{
				myvar.everyCheckOutPage.debug = "Please select.";
			}
			else
			{
				myvar.everyCheckOutPage.selectedCheckOut.submitLeave(false);
			}
		}

		private void leaveAllButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.everyCheckOutPage.debug = "";
			myvar.everyCheckOutPage.submitLeaveAll();
		}

		private void todayButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.everyCheckOutPage.enableFilter = false;
			datePicker1.SelectedDate = DateTime.Today;
			datePicker2.SelectedDate = DateTime.Today;
			myvar.everyCheckOutPage.enableFilter = true;
			searchTextBox.Text = "";
		}
	}
}
