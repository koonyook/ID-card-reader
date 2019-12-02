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
	public partial class TemporaryPage : Page
	{
		private LoadOperation<Identifier> lo1;
		private LoadOperation<IdentifierImage> lo2;
		private LoadOperation<Outsider> lo3;
		private LoadOperation<Outsider_Group> lo4;
		private LoadOperation<Group> lo5;
		private LoadOperation<Coming> lo6;

		private AddPersonWindow addWindow;
		private AddPeopleWindow camWindow;

		public TemporaryPage()
		{
			InitializeComponent();
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (User.permission == User.viewerPermissionName)
			{
				NavigationService.Navigate(new Uri("/ErrorPage", UriKind.Relative));
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			lo1 = myvar.domainContext.Load(myvar.domainContext.GetIdentifiersQuery());
			lo1.Completed += waitForComplete;
			lo2 = myvar.domainContext.Load(myvar.domainContext.GetIdentifierImagesQuery());
			lo2.Completed += waitForComplete;
			lo3 = myvar.domainContext.Load(myvar.domainContext.GetOutsidersQuery());
			lo3.Completed += waitForComplete;
			lo4 = myvar.domainContext.Load(myvar.domainContext.GetOutsider_GroupQuery());
			lo4.Completed += waitForComplete;
			lo5 = myvar.domainContext.Load(myvar.domainContext.GetGroupsQuery());
			lo5.Completed += waitForComplete;
			lo6 = myvar.domainContext.Load(myvar.domainContext.GetComingsQuery());
			lo6.Completed += waitForComplete;
			myvar.busyBinding.update();
			//debug.Text = "ddddd";
		}

		public void waitForComplete(object sender, EventArgs e)
		{
			if (lo1.IsComplete
				&& lo2.IsComplete
				&& lo3.IsComplete
				&& lo4.IsComplete
				&& lo5.IsComplete
				&& lo6.IsComplete)
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
			myvar.tradeTemporaryPage = new TradeTemporaryPage(this);

			//if must load something in loadGroup_Completed method
			//something in tradeTemporaryPage will be unconstruct
			//please use it carefully (**concludeTemporaryCardID)
			this.LayoutRoot.DataContext = myvar.tradeTemporaryPage;
			
			myvar.busyBinding.update();
		}

		private void newPersonButton_Click(object sender, RoutedEventArgs e)
		{
			addWindow = new AddPersonWindow();
			addWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(addWindow_Closing);
			addWindow.Show();
		}

		void addWindow_Closing(object sender, EventArgs e)
		{
			if (addWindow.DialogResult == true)
			{
				myvar.tradeTemporaryPage.temporaryList.Add(new Temporary(
					addWindow.person.cardID,
					addWindow.person.cardIdentifierType,
					addWindow.person.haveCopy,
					myvar.tradeTemporaryPage));
				myvar.tradeTemporaryPage.updateTemporaryCardID();
			}
		}

		private void addPeopleButton_Click(object sender, RoutedEventArgs e)
		{
			camWindow = new AddPeopleWindow();
			camWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(camWindow_Closing);
			camWindow.Show();
		}

		void camWindow_Closing(object sender, EventArgs e)
		{
			if (camWindow.DialogResult == true)
			{
				//add from xml data (myvar...)
				myvar.tradeTemporaryPage.addTemporaryListFromXml(myvar.xmlString);
			}
		}

		private void discardButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.tradeTemporaryPage.selectedTemporary != null)
			{
				myvar.tradeTemporaryPage.temporaryList.Remove(myvar.tradeTemporaryPage.selectedTemporary);
				myvar.tradeTemporaryPage.updateTemporaryCardID();
			}
			else
				MessageBox.Show("Please select.");
		}

		private void submitButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.tradeTemporaryPage.debug = "";
			myvar.tradeTemporaryPage.submit();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{

		}

		private void findNextBlacklistButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.tradeTemporaryPage.temporaryList.Count > 0)
			{
				int startIndex;
				if (myvar.tradeTemporaryPage.selectedTemporary != null)
				{
					startIndex = (listBox1.SelectedIndex + 1) % myvar.tradeTemporaryPage.temporaryList.Count;
				}
				else
				{
					startIndex = 0;
				}
				bool isFirst = true;
				int runner = startIndex;
				while (runner != startIndex || isFirst)
				{
					if ((listBox1.Items[runner] as Temporary).isBlack())
					{
						myvar.tradeTemporaryPage.selectedTemporary = (listBox1.Items[runner] as Temporary);
						return;
					}
					runner = (runner + 1) % myvar.tradeTemporaryPage.temporaryList.Count;
					isFirst = false;
				}
			}
		}

		private void findNextExpireButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.tradeTemporaryPage.temporaryList.Count > 0)
			{
				int startIndex;
				if (myvar.tradeTemporaryPage.selectedTemporary != null)
				{
					startIndex = (listBox1.SelectedIndex + 1) % myvar.tradeTemporaryPage.temporaryList.Count;
				}
				else
				{
					startIndex = 0;
				}
				bool isFirst = true;
				int runner = startIndex;
				while (runner != startIndex || isFirst)
				{
					if ((listBox1.Items[runner] as Temporary).getRestDay() < 0)
					{
						myvar.tradeTemporaryPage.selectedTemporary = (listBox1.Items[runner] as Temporary);
						return;
					}
					runner = (runner + 1) % myvar.tradeTemporaryPage.temporaryList.Count;
					isFirst = false;
				}
			}
		}

		
	}
}
