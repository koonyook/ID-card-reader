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
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RealApplication01.Web;
using System.Windows.Media.Imaging;
using System.Linq;
using RealApplication01.Views;
using System.Xml;
using System.IO;
using System.ServiceModel.DomainServices.Client;

namespace RealApplication01
{
	public class CheckInVisitorPage : INotifyPropertyChanged
	{
		public static bool isDeliver=false;

		public DateTime createTime;

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		//adding 22/5/54
		public void updateIndex()
		{
			foreach (CheckInVisitor man in checkInVisitorList)
			{
				man.OnPropertyChanged(new PropertyChangedEventArgs("index"));
			}
		}

		private string Debug="";
		public string debug
		{
			get { return Debug; }
			set
			{
				Debug = value;
				OnPropertyChanged(new PropertyChangedEventArgs("debug"));
			}
		}

		public VisitorPage parent;
		public CheckInVisitorPage(VisitorPage _parent)
		{
			parent = _parent;
			checkInVisitorList = new ObservableCollection<CheckInVisitor>();
			createTime = DateTime.Now;
		}

		private CheckInVisitor SelectedVisitor = null;	//add last
		public CheckInVisitor selectedVisitor
		{
			get
			{
				return SelectedVisitor;
			}
			set
			{
				SelectedVisitor = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedVisitor"));
			}
		}

		private ObservableCollection<CheckInVisitor> CheckInVisitorList;
		public ObservableCollection<CheckInVisitor> checkInVisitorList 
		{
			get
			{
				return CheckInVisitorList;
			}
			set
			{
				CheckInVisitorList = value;
				OnPropertyChanged(new PropertyChangedEventArgs("checkInVisitorList"));
			}
		}

		public string concludeString
		{
			get
			{
				if (checkInVisitorList == null)
				{
					return "Loading...";
				}
				//things that must to conclude
				int allCount = checkInVisitorList.Count;
				int blackCount = 0;
				foreach (CheckInVisitor man in checkInVisitorList)
				{
					if (man.isBlack())
					{
						blackCount++;
					}
				}
				string ret = allCount + " People";
				if (blackCount > 0)
					ret += ", " + blackCount + " Blacklist";
				return ret;
			}
		}

		public void addVisitorListFromXml(string xmlString)
		{
			XmlReader reader = XmlReader.Create(new StringReader(xmlString));

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "card")
				{
					string cardID = reader.GetAttribute("ID");
					string filename = reader.GetAttribute("filename");
					CheckInVisitor newVisitor = new CheckInVisitor(
						cardID,
						new BitmapImage(new Uri(App.Current.Host.Source, "../images/" + filename)),
						parent.visitorListBox,
						this);
					newVisitor.isDeliver = CheckInVisitorPage.isDeliver;
					this.checkInVisitorList.Add(newVisitor);
				}
			}
			OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
		}

		private Renamer renamer;

		public void submit()
		{
			if (checkInVisitorList.Count == 0)
				return;

			bool haveError = false;
			foreach (CheckInVisitor man in checkInVisitorList)
			{
				if (!man.validate())
				{
					man.borderColor = "Red";
					haveError = true;
				}
				else
				{
					man.borderColor = "Silver";
				}
			}

			if (haveError)
			{
				//rearrange the order of temporaryList
				//bring red to the front(sort by color)
				List<CheckInVisitor> tmpList = checkInVisitorList.ToList<CheckInVisitor>();
				tmpList.Sort((a, b) => string.Compare(a.borderColor, b.borderColor));
				checkInVisitorList = new ObservableCollection<CheckInVisitor>(tmpList);
			}

			//check for the redundant identifier
			foreach (CheckInVisitor manA in checkInVisitorList)
			{
				foreach (CheckInVisitor manB in checkInVisitorList)
				{
					if ((manA != manB)
						&&
						Validater.sameOutsider(manA.identifierID, manA.identifierTypeID, manB.identifierID, manB.identifierTypeID)
					)
					{
						debug += "redundant identifier.\n";
						haveError = true;
						break;
					}
				}
				if (haveError)
					break;
			}
			if (haveError)
			{
				//MessageBox.Show("Please fix data error in red box.");
				return;
			}
			else
			{
				//first rename image to a make sense name
				// real_identifierTypeID_identifierID_(Datetime replace with "_")
				renamer = new Renamer();
				foreach(CheckInVisitor person in checkInVisitorList)
				{
					if(person.imageSource!=null)
					{
						string oldname=person.imageSource.UriSource.ToString().Split('/').Last();
						if (oldname.StartsWith("temp") && person.newFileName == "")		//rename temp image only
						{
							person.newFileName="real_"+person.identifierTypeID.ToString()+"_"+person.identifierID+"("+DateTime.Now.ToString().Replace('/','_').Replace(':','_').Replace(' ','_')+").jpg";
							renamer.namelist.Add(new NamePair(
								oldname,
								person.newFileName
							));
						}
					}
				}
				renamer.onCompleted += rename_Complete;
				renamer.rename();
				//then make change with database (in other method : rename_Complete
			}
		}

		private void rename_Complete(object sender, EventArgs e)
		{
			renamer.onCompleted -= rename_Complete;
			debug = (string)sender;
			//uncomment for enable submitchange
			
			debug += "before makeChange()";
			foreach (CheckInVisitor man in checkInVisitorList)
			{
				man.makeChange();
			}
			//if (myvar.domainContext.HasChanges)
			debug += "before submit";
			myvar.domainContext.SubmitChanges(this.submitVisitorPage_Completed, null);
			
			myvar.busyBinding.update();
		}

		void submitVisitorPage_Completed(SubmitOperation so)
		{
			myvar.busyBinding.update();
			if (so.HasError)
			{
				debug = so.Error.Message + "\n" + so.EntitiesInError.ToString();
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
				this.parent.NavigationService.Navigate(new Uri("/Home", UriKind.Relative));
			}
			//throw new NotImplementedException();
		}
	}

	public class CheckInVisitor : INotifyPropertyChanged
	{
		public CheckInVisitorPage parent;
		private DateTime createTime
		{
			get
			{
				return parent.createTime;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
		//constructor for other type of identifier(do not have image)
		public CheckInVisitor(string _identifierID, IdentifierType _identifierType, bool _haveCopy, ListBox _parentListbox, CheckInVisitorPage _parent)
		{
			this.parent = _parent;

			this.parentListbox = _parentListbox;

			this.identifierType = _identifierType;
			this.IdentifierID = _identifierID;
			
			this.haveCopy = _haveCopy;
			
			this.CanEditID = false;
		}

		//constructor for ThaiCitizenCard
		public CheckInVisitor(string _identifierID, BitmapImage _imageSource, ListBox _parentListBox, CheckInVisitorPage _parent)
		{
			this.parent = _parent;

			this.identifierType = myvar.identifierTypeList[0];	//it's ThaiCitizenCard

			this.parentListbox = _parentListBox;
			this.identifierID = _identifierID;
			this.imageSource = _imageSource;
			
			this.haveCopy = true; //it's image

			this.CanEditID = true;
		}

		private IdentifierType identifierType;
		public string identifierTypeName
		{
			get
			{
				return identifierType.Name;
			}
		}
		public short identifierTypeID
		{
			get
			{
				return identifierType.IdentifierTypeID;
			}
		}

		private bool haveCopy;

		public ListBox parentListbox { get; set; }
		
		public int index
		{
			get
			{
				return parentListbox.Items.IndexOf(this);
			}
		}

		public ObservableCollection<Company> companyList
		{
			get
			{
				return myvar.companyList;
			}
		}
		public ObservableCollection<v_Section> sectionList
		{
			get
			{
				return myvar.sectionList;
			}
		}
		public ObservableCollection<v_Employee> employeeList
		{
			get
			{
				return myvar.employeeList;
			}
		}

		private string IdentifierID;
		public string identifierID 
		{ 
			get
			{
				return IdentifierID;
			}
			set
			{
				IdentifierID = value;
				OnPropertyChanged(new PropertyChangedEventArgs("identifierID"));
				OnPropertyChanged(new PropertyChangedEventArgs("safetyStatus"));
				parent.OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
			}	
		}

		private bool CanEditID;
		public bool canEditID
		{
			get
			{
				return CanEditID;
			}
			set
			{
				CanEditID = value;
				OnPropertyChanged(new PropertyChangedEventArgs("canEditID"));
			}
		}

		public string newFileName="";

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
		
		private Company SelectedCompany;
		public Company selectedCompany
		{
			get
			{
				return SelectedCompany;
			}
			set
			{
				SelectedCompany = value;
				companyPhoneNo = SelectedCompany.TelephoneNo;
				OnPropertyChanged(new PropertyChangedEventArgs("isOtherCompanySelected"));
				OnPropertyChanged(new PropertyChangedEventArgs("selectedCompany"));
			}
		}

		public bool isOtherCompanySelected
		{
			get
			{
				if (SelectedCompany == null || SelectedCompany.CompanyID != -1)
					return false;
				else
					return true;
			}
		}
		private string OtherCompany="";
		public string otherCompany
		{
			get
			{
				//return SelectedCompany.Name;
				return OtherCompany;
			}
			set
			{
				OtherCompany = value;
				OnPropertyChanged(new PropertyChangedEventArgs("otherCompany"));
			}
		}

		private string CompanyPhoneNo = "";
		public string companyPhoneNo
		{
			get
			{
				return CompanyPhoneNo;
			}
			set
			{
				if (value == null)
					CompanyPhoneNo = "";
				else
				{
					string tmp = "";
					//screen for digit only
					for (int i = 0; i < value.Length; i++)
					{
						if (value[i] >= '0' && value[i] <= '9')
							tmp += value[i];
					}
					CompanyPhoneNo = tmp;
				}
				OnPropertyChanged(new PropertyChangedEventArgs("companyPhoneNo"));
			}
		}

		//private int selectedRadioButton=0;	//0=non-select, 1,2,3 = select(section,person,else)

		private bool IsSectionSelected;
		private bool IsPersonSelected;
		private bool IsElseSelected;

		public bool isSectionSelected
		{
			get
			{
				return IsSectionSelected;
			}
			set
			{
				IsSectionSelected = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isSectionSelected"));
			}
		}
		public bool isPersonSelected
		{
			get
			{
				return IsPersonSelected;
			}
			set
			{
				IsPersonSelected = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isPersonSelected"));
			}
		}
		public bool isElseSelected
		{
			get
			{
				return IsElseSelected;
			}
			set
			{
				IsElseSelected = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isElseSelected"));
			}
		}
		
		
		private v_Section SelectedSection;
		public v_Section selectedSection
		{
			get
			{
				return SelectedSection;
			}
			set
			{
				SelectedSection = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedSection"));
			}
		}

		private v_Employee SelectedPerson;
		public v_Employee selectedPerson
		{
			get
			{
				return SelectedPerson;
			}
			set
			{
				SelectedPerson = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedPerson"));
			}
		}

		private string ContactElseText="";
		public string contactElseText
		{
			get
			{
				return ContactElseText;
			}
			set
			{
				ContactElseText = value;
				OnPropertyChanged(new PropertyChangedEventArgs("contactElseText"));
			}
		}

		private string PlateNo;
		public string plateNo
		{
			get
			{
				return PlateNo;
			}
			set
			{
				PlateNo = value;
				OnPropertyChanged(new PropertyChangedEventArgs("plateNo"));
			}
		}

		private bool IsDeliver;
		public bool isDeliver
		{
			get
			{
				return IsDeliver;
			}
			set
			{
				IsDeliver = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isDeliver"));
			}
		}

		public string safetyStatus
		{
			get
			{
				if (isBlack())
					return "Blacklist";
				else
					return "Safe";
			}
		}

		public bool isBlack()
		{
			foreach (v_BlackIdentifierList man in myvar.blackIdentifierList)
			{
				if (
					(	//check same type
						man.IdentifierTypeID == this.identifierType.IdentifierTypeID
						||
						(man.IdentifierTypeID <= 3 && man.IdentifierTypeID <= 3)
					)
					&& man.IdentifierID == this.IdentifierID
				)
					return true;
			}
			return false;
		}

		private string BorderColor = "Silver";
		public string borderColor
		{
			get
			{
				return BorderColor;
			}
			set
			{
				BorderColor = value;
				OnPropertyChanged(new PropertyChangedEventArgs("borderColor"));
			}
		}

		public bool validate()
		{
			if (!Validater.pass(identifierType, IdentifierID))
			{
				//MessageBox.Show("1");
				return false;
			}

			if ((IsSectionSelected || IsPersonSelected || IsElseSelected) == false)
			{
				//MessageBox.Show("2");
				return false;
			}

			if (IsSectionSelected && SelectedSection == null)
			{
				//MessageBox.Show("3");
				return false;
			}

			if (IsPersonSelected && SelectedPerson == null)
			{
				//MessageBox.Show("4");
				return false;
			}

			if (IsElseSelected && ContactElseText == "")
			{
				//MessageBox.Show("5");
				return false;
			}

			if (isOtherCompanySelected && OtherCompany != "" && CompanyPhoneNo == "")
			{
				//fill the company name but not fill the phone number.
				return false;
			}

			return true;
		}

		public void makeChange()
		{
			//insert manythings to domainContext but not submit
			//before make change, user must load identifier and outsider
			Identifier iden = myvar.domainContext.Identifiers.FirstOrDefault(item => item.IdentifierID == this.IdentifierID && item.IdentifierType == this.identifierType);

			if (iden == null)
			{
				iden = new Identifier();

				iden.IdentifierTypeID = this.identifierType.IdentifierTypeID;	//this line error if i assign IdentifierType directly
				//MessageBox.Show("isnull:" + this.haveCopy);
				iden.IdentifierID = this.IdentifierID;

				iden.HaveCopy = this.haveCopy;

				if (this.identifierType.IdentifierTypeID <= 3)	//in group that can map with citizen id
				{
					Identifier idenAgent = myvar.domainContext.Identifiers.FirstOrDefault(item => item.IdentifierID == this.IdentifierID && item.IdentifierTypeID <= 3);
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
			else
			{
				iden.HaveCopy |= this.haveCopy;		
			}
			
			if (this.ImageSource != null && this.newFileName!="")
			{
				IdentifierImage img = new IdentifierImage();
				img.Identifier = iden;
				img.FileName = this.newFileName;
				img.SnapDateTime = this.createTime;
				img.IsCropped = true;

				myvar.domainContext.IdentifierImages.Add(img);
			}

			Company com;	//use for add in Coming
			if (isOtherCompanySelected && OtherCompany!="")
			{
				com = myvar.domainContext.Companies.FirstOrDefault(item => item.Name == this.OtherCompany || item.TelephoneNo==this.CompanyPhoneNo);
				if (com == null)
				{
					com = new Company();
					com.Name = this.OtherCompany;
					com.TelephoneNo = this.CompanyPhoneNo;

					myvar.domainContext.Companies.Add(com);  //must add for other can use
				}
			}
			else if (isOtherCompanySelected == false)
			{
				com = this.SelectedCompany;
			}
			else   //in the case that isOtherCompanySelected==true but OtherCompany==""
			{
				com = null;
			}

			Coming ma = new Coming();
			ma.Outsider = iden.Outsider;
			ma.TimeIn = this.createTime;
			ma.PlateNo = this.PlateNo;
			ma.IsDeliver = this.IsDeliver;
			ma.IsKickedOut = false;
			ma.Inspector = User.username;
			ma.ComputerName = User.computername;
			if(com!=null)
				ma.Company = com;
			if (IsSectionSelected)
			{
				ma.ContactSectionID = this.SelectedSection.FuncID;
			}
			if (IsPersonSelected)
			{
				ma.ContactEMPID = this.SelectedPerson.EMPID;
			}
			if (IsElseSelected)
			{
				ma.ContactElse = this.ContactElseText;
			}
		}
	}
}
