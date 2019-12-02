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
using System.ComponentModel;

namespace RealApplication01.Views
{
	public partial class ContractorPage : Page
	{
		private AddPersonWindow addWindow;

		private AddPeopleWindow camWindow;

		public ContractorPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			//myvar.loadIdentifier = myvar.domainContext.Load(myvar.domainContext.GetIdentifiersQuery());
			//myvar.loadIdentifier.Completed += myvar.updateBusyBinding;

			//myvar.loadOutsider = myvar.domainContext.Load(myvar.domainContext.GetOutsidersQuery());
			//myvar.loadOutsider.Completed += myvar.updateBusyBinding;

			myvar.domainContext.Load(myvar.domainContext.GetIdentifiersWithOutsidersAndImagesQuery()
				, loadIdentifiersWithOutsidersAndImages_Completed, null);

			myvar.busyBinding.update();
		}


		public void loadIdentifiersWithOutsidersAndImages_Completed(LoadOperation<Identifier> lo)//(object sender, EventArgs e)
		{
			//load data to myvar.regisContractorPage
			myvar.regisContractorPage = new RegisContractorPage(this);

			this.LayoutRoot.DataContext = myvar.regisContractorPage;
			
			myvar.busyBinding.update();
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (User.permission == User.viewerPermissionName)
			{
				userControl1.IsEnabled = false;
				passAllButton.IsEnabled = false;
				submitButton.IsEnabled = false;
			}
		}

		private void newPersonButton_Click(object sender, RoutedEventArgs e)
		{
			addWindow = new AddPersonWindow();
			//addWindow.cardTypeComboBox.ItemsSource = myvar.identifierTypeList;
			addWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(addWindow_Closing);
			addWindow.Show();
		}

		void addWindow_Closing(object sender, EventArgs e)
		{
			if (addWindow.DialogResult == true)
			{
				myvar.regisContractorPage.contractorList.Add(new Contractor(
					addWindow.person.cardID,
					addWindow.person.cardIdentifierType,
					addWindow.person.haveCopy,
					myvar.regisContractorPage));

				myvar.regisContractorPage.OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
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
				myvar.regisContractorPage.addContractorListFromXml(myvar.xmlString);
			}
		}

		private void discardButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.regisContractorPage.selectedContractor != null)
			{
				myvar.regisContractorPage.contractorList.Remove(myvar.regisContractorPage.selectedContractor);
				myvar.regisContractorPage.OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
			}
			else
				MessageBox.Show("Please select.");
		}

		private void submitButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.regisContractorPage.debug = "";
			myvar.regisContractorPage.submit();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			//nothing
		}

		private void passAllButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (Contractor man in myvar.regisContractorPage.contractorList)
			{
				man.isPassed = true;
			}
		}

		private void findNextBlacklistButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.regisContractorPage.contractorList.Count > 0)
			{
				int startIndex;
				if (myvar.regisContractorPage.selectedContractor != null)
				{
					startIndex = (listBox1.SelectedIndex + 1) % myvar.regisContractorPage.contractorList.Count;
				}
				else
				{
					startIndex = 0;
				}
				bool isFirst = true;
				int runner = startIndex;
				while (runner != startIndex || isFirst)
				{
					if ((listBox1.Items[runner] as Contractor).isBlack())
					{
						myvar.regisContractorPage.selectedContractor = (listBox1.Items[runner] as Contractor);
						return;
					}
					runner = (runner + 1) % myvar.regisContractorPage.contractorList.Count;
					isFirst = false;
				}
			}
		}

		private void searchButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.regisContractorPage.contractorList.Count > 0)
			{
				int startIndex;
				if (myvar.regisContractorPage.selectedContractor != null)
				{
					startIndex = (listBox1.SelectedIndex + 1) % myvar.regisContractorPage.contractorList.Count;
				}
				else
				{
					startIndex = 0;
				}
				bool isFirst = true;
				int runner = startIndex;
				while (runner != startIndex || isFirst)
				{
					if ((listBox1.Items[runner] as Contractor).isMatched(searchTextBox.Text))
					{
						myvar.regisContractorPage.selectedContractor = (listBox1.Items[runner] as Contractor);
						return;
					}
					runner = (runner + 1) % myvar.regisContractorPage.contractorList.Count;
					isFirst = false;
				}
			}
		}

		private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				searchButton_Click(null, null);
			}
		}

		

	}
}
