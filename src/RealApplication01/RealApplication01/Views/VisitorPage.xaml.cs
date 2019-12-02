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
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using System.Windows.Media.Imaging;
using RealApplication01.Web;
using System.ServiceModel.DomainServices.Client;
using System.ComponentModel;

namespace RealApplication01.Views
{
    public partial class VisitorPage : Page
    {
		
		private AddPersonWindow addWindow;

		private AddPeopleWindow camWindow;


        public VisitorPage()
        {
            InitializeComponent();
			
        }

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			myvar.loadIdentifier = myvar.domainContext.Load(myvar.domainContext.GetIdentifiersQuery());
			myvar.loadIdentifier.Completed += myvar.updateBusyBinding;

			myvar.loadOutsider = myvar.domainContext.Load(myvar.domainContext.GetOutsidersQuery());
			myvar.loadOutsider.Completed += myvar.updateBusyBinding;

			myvar.busyBinding.update();
		}

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			if (User.permission == User.viewerPermissionName)
			{
				NavigationService.Navigate(new Uri("/ErrorPage", UriKind.Relative));
				return;
			}

			myvar.checkInVisitorPage = new CheckInVisitorPage(this);
			this.LayoutRoot.DataContext = myvar.checkInVisitorPage;
			myvar.checkInVisitorPage.addVisitorListFromXml(myvar.xmlString);
			
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
				myvar.checkInVisitorPage.checkInVisitorList.Add(new CheckInVisitor(
					addWindow.person.cardID,
					addWindow.person.cardIdentifierType,
					addWindow.person.haveCopy,
					visitorListBox,
					myvar.checkInVisitorPage));
				myvar.checkInVisitorPage.OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
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
				myvar.checkInVisitorPage.addVisitorListFromXml(myvar.xmlString);
			}
		}

		private void button3_Click(object sender, RoutedEventArgs e)
		{
			//myvar.checkInVisitorList[0].identifierID = "48934";
			//myvar.checkInVisitorList.Add(new CheckInVisitor("99",visitorListBox));
		}

		private void discardButton_Click(object sender, RoutedEventArgs e)
		{
			if (visitorListBox.SelectedIndex != -1)
			{
				myvar.checkInVisitorPage.checkInVisitorList.Remove(visitorListBox.SelectedItem as CheckInVisitor);
				myvar.checkInVisitorPage.updateIndex();
				myvar.checkInVisitorPage.OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
			}
			else
				MessageBox.Show("Please select.");
		}

		private void copyButton_Click(object sender, RoutedEventArgs e)
		{
			//copy data from selected item to others
			if (visitorListBox.SelectedIndex != -1)
			{
				CheckInVisitor source = visitorListBox.SelectedItem as CheckInVisitor;
				/*
				MessageBox.Show(source.isSectionSelected.ToString()+
					source.isPersonSelected.ToString()+
					source.isElseSelected.ToString());
				*/
				foreach (CheckInVisitor man in myvar.checkInVisitorPage.checkInVisitorList)
				{
					if (man != source)
					{
						if(source.selectedCompany!=null)
							man.selectedCompany = source.selectedCompany;
						
						man.otherCompany = source.otherCompany;
						man.companyPhoneNo = source.companyPhoneNo;
						
						man.plateNo = source.plateNo;
						man.isDeliver = source.isDeliver;

						man.isSectionSelected = source.isSectionSelected;
						man.isPersonSelected = source.isPersonSelected;
						man.isElseSelected = source.isElseSelected;

						if(source.selectedSection!=null)
							man.selectedSection = source.selectedSection;
						if(source.selectedPerson!=null)
							man.selectedPerson = source.selectedPerson;
						if(source.contactElseText!=null)
							man.contactElseText = source.contactElseText;
					}
				}
				
			}
			else
				MessageBox.Show("Please select.");
		}

		private void submitButton_Click(object sender, RoutedEventArgs e)
		{
			myvar.checkInVisitorPage.debug = "";
			myvar.checkInVisitorPage.submit();			
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(
				myvar.checkInVisitorPage.checkInVisitorList[0].imageSource.UriSource.ToString());
		}

		private void findNextBlacklistButton_Click(object sender, RoutedEventArgs e)
		{
			if (myvar.checkInVisitorPage.checkInVisitorList.Count > 0)
			{
				int startIndex;
				if (myvar.checkInVisitorPage.selectedVisitor != null)
				{
					startIndex = (visitorListBox.SelectedIndex + 1) % myvar.checkInVisitorPage.checkInVisitorList.Count;
				}
				else
				{
					startIndex = 0;
				}
				bool isFirst = true;
				int runner = startIndex;
				while (runner != startIndex || isFirst)
				{
					if ((visitorListBox.Items[runner] as CheckInVisitor).isBlack())
					{
						myvar.checkInVisitorPage.selectedVisitor = (visitorListBox.Items[runner] as CheckInVisitor);
						return;
					}
					runner = (runner + 1) % myvar.checkInVisitorPage.checkInVisitorList.Count;
					isFirst = false;
				}
			}
		}
    }
}
