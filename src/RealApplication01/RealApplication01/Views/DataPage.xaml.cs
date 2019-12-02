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
using RealApplication01.Web;

namespace RealApplication01.Views
{
	public partial class DataPage : Page
	{
		private AddBlackEventWindow addWindow;

		public DataPage()
		{
			InitializeComponent();
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (User.permission == User.guardPermissionName 
				|| User.permission == User.viewerPermissionName)
			{
				dateDatePicker.IsEnabled = false;
				typeTextBox.IsEnabled = false;
				detailTextBox.IsEnabled = false;
				addBlackEventButton.IsEnabled = false;
				deleteBlackEventButton.IsEnabled = false;
				editBlackEventButton.IsEnabled = false;
				rejectButton.IsEnabled = false;
			}
		}

		//this method is workaround for make true binding for bool query parameter
		private void blackListOnlyCheckBox_CheckedChange(object sender, RoutedEventArgs e)
		{
			outsiderDomainDataSource.QueryParameters[0].Value = blackListOnlyCheckBox.IsChecked;
			outsiderDomainDataSource.Load();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			//Outsider x;
			//outsiderDomainDataSource1.da
			textBox1.Text = (identifierImageDomainDataSource.Data as IEnumerable<IdentifierImage>).ToString();
		}

		private void outsiderDomainDataSource_LoadedData(object sender, LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void button1_Click_1(object sender, RoutedEventArgs e)
		{
			
		}

		private void outsiderDomainDataSource1_LoadedData(object sender, LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void outsiderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//comingsDataGrid.ItemsSource = (outsiderDataGrid.SelectedItem)
			identifiersDataGrid.SelectedIndex=0;
		}

		private void outsiderDomainDataSource1_LoadedData_1(object sender, LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void identifierImageDomainDataSource_LoadedData(object sender, LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void identifiersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			identifierImageDomainDataSource.Load();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			outsiderDomainDataSource.Load();
		}

		private void todayOnlyCheckBox_CheckedChange(object sender, RoutedEventArgs e)
		{
			outsiderDomainDataSource.QueryParameters[1].Value = todayOnlyCheckBox.IsChecked;
			outsiderDomainDataSource.Load();
		}

		private void outsiderDomainDataSource1_LoadedData_2(object sender, LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void editBlackEventButton_Click(object sender, RoutedEventArgs e)
		{
			outsiderDomainDataSource.SubmitChanges();
		}

		private void rejectButton_Click(object sender, RoutedEventArgs e)
		{
			outsiderDomainDataSource.RejectChanges();
		}

		private void addBlackEventButton_Click(object sender, RoutedEventArgs e)
		{
			addWindow = new AddBlackEventWindow();
			//addWindow.cardTypeComboBox.ItemsSource = myvar.identifierTypeList;
			addWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(addWindow_Closing);
			addWindow.Show();
		}

		void addWindow_Closing(object sender, EventArgs e)
		{
			if (addWindow.DialogResult == true)
			{
				//reload blacklist in myvar that must to update
				myvar.domainContext.Load(myvar.domainContext.GetV_BlackIdentifierListQuery(), myvar.loadBlackIdentifier_Completed, null);
				//reload the DataPage
				outsiderDomainDataSource.Load();
				myvar.busyBinding.update();
			}
		}

		private void deleteBlackEventButton_Click(object sender, RoutedEventArgs e)
		{
			if (blackEventsDataGrid.SelectedItem != null)
			{
				if(MessageBox.Show("คุณต้องการลบข้อมูลนี้ใช่หรือไม่?","คำยืนยัน", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
					var cat = outsiderDomainDataSource.DomainContext as DomainService1;
					cat.BlackEvents.Remove(blackEventsDataGrid.SelectedItem as BlackEvent);
					outsiderDomainDataSource.SubmitChanges();
				}
			}
			else
			{
				MessageBox.Show("Please select a black event.");
			}
		}

		private void searcherTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				outsiderDomainDataSourceLoadButton.Focus();
				outsiderDomainDataSource.Load();
			}
		}
	}
}
