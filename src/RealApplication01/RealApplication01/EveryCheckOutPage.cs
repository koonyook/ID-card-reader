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
using RealApplication01.Web;
using System.Windows.Media.Imaging;
using System.Linq;
using RealApplication01.Views;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;

namespace RealApplication01
{
	public class EveryCheckOutPage : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

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

		public CheckOutPage parent;

		public EveryCheckOutPage(CheckOutPage _parent)
		{
			//must load everything before call constructor
			parent = _parent;

			SelectedFromDate = DateTime.Today;
			SelectedToDate = DateTime.Today;

			checkOutList = new ObservableCollection<CheckOut>();
			IEnumerable<Coming> canCheckOutList = myvar.domainContext.Comings.Where<Coming>(item =>
				//item.TimeIn.Date == DateTime.Today &&
				item.TimeOut == null	//must screen again because of using with others module
				);
			foreach (Coming man in canCheckOutList)
			{
				CheckOut newCheckOut = new CheckOut(man,this);
				this.checkOutList.Add(newCheckOut);
			}
			
			OnPropertyChanged(new PropertyChangedEventArgs("checkOutList"));
				
				
			myvar.busyBinding.update();
			filter();
		}
		
		public string concludeTemporaryCardID
		{
			get
			{
				int backupCount = 0;
				int visitorCount = 0;
				List<short> use = new List<short>();
				foreach (CheckOut card in checkOutList)
				{
					if (card.isVisible)
					{
						if (card.coming.TemporaryCardID == null)
						{
							visitorCount++;
						}
						else if (card.coming.TemporaryCardID == 0)
						{
							backupCount++;
						}
						else if ((card.coming.TemporaryCardID != 0) && (!use.Contains((short)card.coming.TemporaryCardID)))
						{
							use.Add((short)card.coming.TemporaryCardID);
						}
					}
				}
				use.Sort();
				int start=-1, end=-1;
				string ret = "";
				if (visitorCount > 0)
				{
					ret += visitorCount.ToString() + "Visitor(s), ";
				}
				if (backupCount > 0)
				{
					ret+=backupCount.ToString() + " backupCard(s), ";
				}
				for (int i = 0; i < use.Count; i++)
				{
					if (i == 0 || use[i] - use[i - 1] > 1)
					{
						start = use[i];
					}
					if (i == use.Count - 1 || use[i + 1] - use[i] > 1)
					{
						end = use[i];
						if (start == end)
						{
							ret += start.ToString() + ", ";
						}
						else if(start < end)
						{

							ret += start.ToString() + "-" + end.ToString() + ", ";
						}
					}
				}
				return ret;
			}
		}

		public ObservableCollection<CheckOut> checkOutList { get; set; }
		
		private CheckOut SelectedCheckOut=null;
		public CheckOut selectedCheckOut
		{
			get
			{
				return SelectedCheckOut;
			}
			set
			{
				SelectedCheckOut = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedCheckOut"));
			}
		}

		public int peopleCount
		{
			get
			{
				int count = 0;
				foreach (CheckOut man in checkOutList)
				{
					if (man.isVisible)
						count++;
				}
				return count;
			}
		}

		private void filter()
		{
			foreach (CheckOut man in checkOutList)
			{
				if (match(man, SearchString, SelectedFromDate.Date , SelectedToDate.Date))
				{
					man.isVisible = true;
				}
				else
				{
					man.isVisible = false;
				}
			}

			List<CheckOut> tmpList = checkOutList.ToList<CheckOut>();
			tmpList.Sort((a, b) =>
				{
					if (a.isVisible == true && b.isVisible == false)
						return -1;
					else if (a.isVisible == false && b.isVisible == true)
						return 1;
					else
					{
						if (a.coming.TimeIn > b.coming.TimeIn)
							return 1;
						else if (a.coming.TimeIn < b.coming.TimeIn)
							return -1;
						else
							return 0;
					}
				}
			);
			checkOutList = new ObservableCollection<CheckOut>(tmpList);

			OnPropertyChanged(new PropertyChangedEventArgs("peopleCount"));
			OnPropertyChanged(new PropertyChangedEventArgs("checkOutList"));
			OnPropertyChanged(new PropertyChangedEventArgs("concludeTemporaryCardID"));
		}
		public bool enableFilter = true;

		private DateTime SelectedFromDate;
		public DateTime selectedFromDate
		{
			get { return SelectedFromDate; }
			set
			{
				/*
				if (value == null)
				{
					throw new Exception("Cannot leave this value be null.");
				}*/
				SelectedFromDate = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedFromDate"));
				if(enableFilter)
					filter();
			}
		}

		private DateTime SelectedToDate;
		public DateTime selectedToDate
		{
			get { return SelectedToDate; }
			set
			{
				/*
				if (value == null)
				{
					throw new Exception("Cannot leave this value be null.");
				}*/
				SelectedToDate = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedToDate"));
				if(enableFilter)
					filter();
			}
		}

		private string SearchString="";
		public string searchString
		{
			get
			{
				return SearchString;
			}
			set
			{
				SearchString = value;
				OnPropertyChanged(new PropertyChangedEventArgs("searchString"));
				if(enableFilter)
					filter();
			}
		}

		private bool match(CheckOut man, string searcher, DateTime dateStart,DateTime dateEnd)
		{
			if (man.coming.TimeIn.Date < dateStart || man.coming.TimeIn.Date > dateEnd)
			{
				return false;
			}

			if (searcher == "")
			{
				//debug += "a";
				return true;
			}
			if (man.identifierID.Contains(searcher))
			{
				//debug += "b";
				return true;
			}
			if (man.companyName.Contains(searcher))
			{
				//debug += "c";
				return true;
			}
			if (man.tempCardID.Contains(searcher))
			{
				//debug += "d";
				return true;
			}
			if (man.checkInTime == searcher)
			{
				//debug += "e";
				return true;
			}
			//debug += "f";
			return false;
		}
		
		public void submitLeaveAll()
		{
			foreach (CheckOut man in checkOutList)
			{
				if (man.isVisible)
				{
					man.coming.TimeOut = DateTime.Now;
				}
			}

			myvar.domainContext.SubmitChanges(this.submitCheckOutPage_Completed, null);
			myvar.busyBinding.update();		
		}
		

		private void submitCheckOutPage_Completed(SubmitOperation so)
		{
			if (so.HasError)
			{
				debug = so.Error.Message + "\n" + so.EntitiesInError.ToString();
				foreach (Object x in so.EntitiesInError)
				{
					debug += x.ToString();
				}
				myvar.domainContext.RejectChanges();
				foreach (CheckOut man in checkOutList)
				{
					if (man.isVisible)
					{
						man.coming.TimeOut = null;
					}
				}
				so.MarkErrorAsHandled();
			}
			else
			{
				debug += "---Finish Leave All---";
				//throw new NotImplementedException();
				List<CheckOut> removeList = new List<CheckOut>();
				foreach (CheckOut man in checkOutList)
				{
					if (man.isVisible)
					{
						removeList.Add(man);
					}
				}
				foreach (CheckOut man in removeList)
				{
					this.checkOutList.Remove(man);
				}
				updateInterface();
			}
			myvar.busyBinding.update();
		}

		public void updateInterface()
		{
			OnPropertyChanged(new PropertyChangedEventArgs("searchString"));
			OnPropertyChanged(new PropertyChangedEventArgs("peopleCount"));
			OnPropertyChanged(new PropertyChangedEventArgs("checkOutList"));
			OnPropertyChanged(new PropertyChangedEventArgs("concludeTemporaryCardID"));
		}
	}

	public class CheckOut : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
		
		public Identifier identifier;
		public Coming coming;
		public EveryCheckOutPage parent;

		//constructor for adding from group
		public CheckOut(Coming _coming, EveryCheckOutPage _parent)
		{
			coming=_coming;
			parent = _parent;

			identifier =coming.Outsider.Identifiers.LastOrDefault<Identifier>(item => item.IdentifierTypeID==1);
			if(identifier==null)
			{
				identifier =coming.Outsider.Identifiers.Last<Identifier>();
			}

			IdentifierImage idenImage = identifier.IdentifierImages.LastOrDefault<IdentifierImage>();
			if(idenImage != null)
			{
				imageSource = new BitmapImage(new Uri(App.Current.Host.Source, "../images/" 
					+ idenImage.FileName));
			}
		}

		public string identifierTypeName
		{
			get
			{
				return identifier.IdentifierType.Name;
			}
		}

		public string identifierID
		{
			get
			{
				return identifier.IdentifierID;
			}
		}

		private BitmapImage ImageSource;
		public BitmapImage imageSource
		{
			get { return ImageSource; }
			set
			{
				ImageSource = value;
				OnPropertyChanged(new PropertyChangedEventArgs("imageSource"));
			}
		}

		public string companyName
		{
			get
			{
				if (coming.Company != null)
					return coming.Company.Name;
				else
					return "null";
			}
		}

		public string plateNo
		{
			get
			{
				if (coming.PlateNo != null)
					return coming.PlateNo;
				else
					return "ไม่มีรถ";
			}
		}

		public string checkInTime
		{
			get
			{
				//return (coming.TimeIn.TimeOfDay.Subtract(TimeSpan.FromMilliseconds(coming.TimeIn.TimeOfDay.Milliseconds)))
				//	.ToString();
				return coming.TimeIn.ToString().Split('.')[0];
			}
		}

		public string tempCardID
		{
			get
			{
				if (coming.TemporaryCardID != null)
				{
					if (coming.TemporaryCardID > 0)
						return "Temp" + coming.TemporaryCardID.ToString();
					else
						return "BackupCard";
				}
				else
				{
					return "Visitor";
				}
			}
		}

		private bool IsVisible=true;
		public bool isVisible
		{
			get
			{
				return IsVisible;
			}
			set
			{
				IsVisible = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isVisible"));
			}
		}

		public void submitLeave(bool isKicked)
		{
			//update coming
			coming.TimeOut = DateTime.Now;
			coming.IsKickedOut = isKicked;
			myvar.domainContext.SubmitChanges(submitCheckOutPage_Completed, null);
			myvar.busyBinding.update();
		}

		private void submitCheckOutPage_Completed(SubmitOperation so)
		{
			
			if (so.HasError)
			{
				parent.debug = so.Error.Message + "\n" + so.EntitiesInError.ToString();
				foreach (Object x in so.EntitiesInError)
				{
					parent.debug += x.ToString();
				}
				myvar.domainContext.RejectChanges();
				coming.TimeOut = null;
				so.MarkErrorAsHandled();
			}
			else
			{
				parent.debug += "---Finish---";
				parent.checkOutList.Remove(this);
				parent.updateInterface();
			}
			//throw new NotImplementedException();
			myvar.busyBinding.update();
		}
	}
}




