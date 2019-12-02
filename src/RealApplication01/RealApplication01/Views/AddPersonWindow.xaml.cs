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
using RealApplication01.Web;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace RealApplication01.Views
{
	public partial class AddPersonWindow : ChildWindow
	{
		public AddPerson person;

		public AddPersonWindow()
		{
			InitializeComponent();

			person = new AddPerson();
			LayoutRoot.DataContext = person;
		}


		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (Validater.pass(((IdentifierType)cardTypeComboBox.SelectedItem).IdentifierTypeID, cardIDTextBox.Text))
			{
				//exit to mainpage
				this.DialogResult = true;
			}
			/*
			else
			{
				 //error
			}
			*/
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void cardTypeComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			cardTypeComboBox.SelectedItem = myvar.identifierTypeList[0];	//this place only (cannot place before this time)
		}

		
	}

	public class AddPerson : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		public AddPerson()
		{
		}

		public ObservableCollection<IdentifierType> identifierTypeList
		{
			get
			{
				return myvar.identifierTypeList;
			}
		}

		private string CardID="";
		public string cardID
		{
			get { return CardID; }
			set
			{
				if (Validater.pass(CardIdentifierType.IdentifierTypeID, value))
				{
					CardID = value;
					OnPropertyChanged(new PropertyChangedEventArgs("cardID"));
				}
				else
				{
					throw new Exception("Wrong ID");
				}
			}
		}
		private IdentifierType CardIdentifierType;
		public IdentifierType cardIdentifierType
		{
			get { return CardIdentifierType; }
			set
			{
				CardIdentifierType = value;
				OnPropertyChanged(new PropertyChangedEventArgs("cardIdentifierType"));
				OnPropertyChanged(new PropertyChangedEventArgs("cardID"));
			}
		}

		public bool haveCopy { get; set; }
	}
}

