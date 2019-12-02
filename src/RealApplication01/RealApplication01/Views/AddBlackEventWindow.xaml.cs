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
using System.ComponentModel;
using System.Collections.ObjectModel;
using RealApplication01.Web;
using System.ServiceModel.DomainServices.Client;

namespace RealApplication01.Views
{
	public partial class AddBlackEventWindow : ChildWindow
	{
		public AddBlackEvent black;

		public AddBlackEventWindow()
		{
			InitializeComponent();

			black = new AddBlackEvent(this);
			LayoutRoot.DataContext = black;
		}


		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			bool pass = true;
			debug.Text = "";
			if (!Validater.pass(((IdentifierType)cardTypeComboBox.SelectedItem).IdentifierTypeID, cardIDTextBox.Text))
			{
				//if (cardIDTextBox.Text == null || cardIDTextBox.Text == "")
				//	cardIDTextBox.Text = "";
				debug.Text += "Please fill ID.\n";
				pass = false;
			}

			if (blackTypeTextBox.Text == null || blackTypeTextBox.Text == "")
			{
				//blackTypeTextBox.Text = "";
				debug.Text+= "Please fill behavior type.";
				pass = false;
			}
			if(datePicker1.SelectedDate==null)
			{
				pass = false;
			}
			if (pass)
			{
				black.submit();
				black.OnPropertyChanged(new PropertyChangedEventArgs("isBusy"));
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void cardTypeComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			cardTypeComboBox.SelectedItem = myvar.identifierTypeList[0];	//this place only (cannot place before this time)
		}

		private void datePicker1_Loaded(object sender, RoutedEventArgs e)
		{
			datePicker1.SelectedDate = DateTime.Today;
		}
	}

	public class AddBlackEvent : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		public AddBlackEventWindow parent;

		private string Debug = "";
		public string debug
		{
			get { return Debug; }
			set
			{
				Debug = value;
				OnPropertyChanged(new PropertyChangedEventArgs("debug"));
			}
		}

		public AddBlackEvent(AddBlackEventWindow _parent)
		{
			parent =_parent;
		}

		public ObservableCollection<IdentifierType> identifierTypeList
		{
			get
			{
				return myvar.identifierTypeList;
			}
		}

		public bool isBusy
		{
			get
			{
				return myvar.busyBinding.isBusy;
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

		private DateTime DateOfEvent;
		public DateTime dateOfEvent
		{
			get { return DateOfEvent; }
			set
			{
				if (value == null)
				{
					throw new Exception("Cannot leave this value be null.");
				}
				DateOfEvent = value;
				OnPropertyChanged(new PropertyChangedEventArgs("dateOfEvent"));
			}
		}

		private string BehaviorType = "";
		public string behaviorType
		{
			get { return BehaviorType; }
			set
			{
				if (value == null || value == "")
				{
					throw new Exception("Cannot leave this value be null.");
				}
				BehaviorType = value;
				OnPropertyChanged(new PropertyChangedEventArgs("behaviorType"));
			}
		}

		private string Detail = "";
		public string detail
		{
			get { return Detail; }
			set
			{
				Detail = value;
				OnPropertyChanged(new PropertyChangedEventArgs("detail"));
			}
		}

		public void submit()
		{
			//make change and submit via myvar.domainContext
			//first load significant data
			myvar.domainContext.Load<Identifier>(myvar.domainContext.GetIdentifiersWithOutsidersFromIdentifierIDQuery(this.CardID), preload_Completed,null);
			debug += "preload ok;";
		}

		public void preload_Completed(LoadOperation<Identifier> lo)//(object sender, EventArgs e)
		{
			debug += "preload complete;";
			Identifier iden = myvar.domainContext.Identifiers.FirstOrDefault(item => this.CardID == item.IdentifierID  && item.IdentifierType==this.CardIdentifierType);

			if (iden == null)
			{
				iden = new Identifier();

				iden.IdentifierTypeID = this.CardIdentifierType.IdentifierTypeID;	//this line error if i assign IdentifierType directly
				//MessageBox.Show("isnull:" + this.haveCopy);
				iden.IdentifierID = this.CardID;

				iden.HaveCopy = this.haveCopy;

				if (this.CardIdentifierType.IdentifierTypeID <= 3)	//in group that can map with citizen id
				{
					Identifier idenAgent = myvar.domainContext.Identifiers.FirstOrDefault(item => item.IdentifierID == this.CardID && item.IdentifierTypeID <= 3);
					//MessageBox.Show("if..."+idenAgent.OutsiderID);
					if (idenAgent == null)
					{
						iden.Outsider = new Outsider();
					}
					else
					{
						iden.Outsider = idenAgent.Outsider;
					}
				}
				else
				{
					//MessageBox.Show("else");
					iden.Outsider = new Outsider();
				}

				myvar.domainContext.Identifiers.Add(iden);
			}

			BlackEvent newevent = new BlackEvent();
			newevent.Date = this.DateOfEvent.Date;
			newevent.Detail = this.Detail;
			newevent.Type = this.BehaviorType;
			newevent.Outsider = iden.Outsider;
			myvar.domainContext.BlackEvents.Add(newevent);
			myvar.domainContext.SubmitChanges(submit_Completed,null);
		}

		public void submit_Completed(SubmitOperation so)
		{
			OnPropertyChanged(new PropertyChangedEventArgs("isBusy"));
			if (so.HasError)
			{
				debug += so.Error.Message + "\n" + so.EntitiesInError.ToString();
				foreach (Object x in so.EntitiesInError)
				{
					debug += x.ToString();
				}
				myvar.domainContext.RejectChanges();
				so.MarkErrorAsHandled();
			}
			else
			{
				debug += "---Finish---";
				parent.DialogResult = true;		//finish
			}
		}
	}
}

