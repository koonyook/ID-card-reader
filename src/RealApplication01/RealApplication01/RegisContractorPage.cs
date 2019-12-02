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
	public class RegisContractorPage : INotifyPropertyChanged
	{
		public static long editGroupID;
		public Group editGroup;
		public DateTime createTime;

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

		public ContractorPage parent;

	
		public RegisContractorPage(ContractorPage _parent)
		{
			parent = _parent;
			if (RegisContractorPage.editGroupID <= 0)		//new group
			{
				contractorList = new ObservableCollection<Contractor>();
				this.addContractorListFromXml(myvar.xmlString);
				createTime = DateTime.Now;
				IsNewGroup = true;
			}
			else      //for Old Group
			{
				IsNewGroup = false;

				//load specific query for getting a Group
				//include with Outsider_Group
				myvar.domainContext.Load(
					myvar.domainContext.GetGroupsWithOutsider_GroupsQuery(RegisContractorPage.editGroupID)
					, loadGroup_Completed, null);
			}

			myvar.busyBinding.update();
		
		}

		public void loadGroup_Completed(LoadOperation<Group> lo)//(object sender, EventArgs e)
		{
			//load data to myvar.regisContractorPage
			
			editGroup = myvar.domainContext.Groups.FirstOrDefault<Group>(item => item.GroupID==RegisContractorPage.editGroupID);
			if (editGroup == null)
			{
				debug = "ERROR:Cannot load that group to edit";
			}
			else
			{
				
				selectedCompany = editGroup.Company;	//this cannnot be null
				
				if (editGroup.EngineerEMPID != null)
				{
					selectedEngineer = myvar.employeeList.FirstOrDefault<v_Employee>(item => item.EMPID == editGroup.EngineerEMPID);
					if (selectedEngineer == null)
						debug = "ERROR: cannot select engineer";
				}
				workingZone = editGroup.WorkArea;
				workingType = editGroup.WorkType;
				Identifier identmp = editGroup.Outsider.Identifiers.FirstOrDefault<Identifier>(item => item.IdentifierTypeID <= 3);
				if (identmp == null)
					debug = "ERROR: cannot load identmp";
				agentIdentifierID = identmp.IdentifierID;
				agentName = editGroup.Outsider.Name;
				agentSName = editGroup.Outsider.SName;
				agentPhone = editGroup.AgentTelephoneNo;
				//companyAddress = editGroup.CompanyAddress;
				emergencyPerson = editGroup.EmergencyContact;
				emergencyPhone = editGroup.EmergencyCallNo;
				createTime = editGroup.TimeIn;

				contractorList = new ObservableCollection<Contractor>();
				//debug = editGroup.Outsider_Group.Count().ToString();
				foreach (Outsider_Group man in editGroup.Outsider_Group)
				{
					Identifier iden = man.Outsider.Identifiers.FirstOrDefault<Identifier>();
					if (iden == null)
						debug = "ERROR: cannot load iden of man";

					Contractor newcon = new Contractor(
						iden.IdentifierID,
						iden.IdentifierType,
						iden.HaveCopy,
						iden.IdentifierImages.LastOrDefault<IdentifierImage>(),		//edit 22/5/54
						man.havePhoto,
						man.isPassed,
						this
					);
					this.contractorList.Add(newcon);
				}
				OnPropertyChanged(new PropertyChangedEventArgs("contractorList"));
				OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
				//debug = this.contractorList.Count().ToString();
			}
			//parent.listBox1.ItemsSource = this.contractorList;
			myvar.busyBinding.update();
		}

		private ObservableCollection<Contractor> ContractorList;
		public ObservableCollection<Contractor> contractorList
		{
			get
			{
				return ContractorList;
			}
			set
			{
				ContractorList = value;
				OnPropertyChanged(new PropertyChangedEventArgs("contractorList"));
			}
		}

		public string concludeString
		{
			get
			{
				if (contractorList == null)
				{
					return "Loading...";
				}
				//things that must to conclude
				int allCount = contractorList.Count;
				int blackCount = 0;
				foreach (Contractor man in contractorList)
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

		private Contractor SelectedContractor=null;
		public Contractor selectedContractor
		{
			get
			{
				return SelectedContractor;
			}
			set
			{
				SelectedContractor = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedContractor"));
			}
		}

		public void addContractorListFromXml(string xmlString)
		{
			XmlReader reader = XmlReader.Create(new StringReader(xmlString));
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "card")
				{
					string cardID = reader.GetAttribute("ID");
					string filename = reader.GetAttribute("filename");

					Contractor newContractor = new Contractor(
						cardID,
						new BitmapImage(new Uri(App.Current.Host.Source, "../images/" + filename)),
						this);
					this.contractorList.Add(newContractor);
				}
			}
			OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
		}

		public ObservableCollection<Company> companyList
		{
			get
			{
				return myvar.companyList;
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

				companyAddress = SelectedCompany.Address;
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

		private string OtherCompany = "";
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

		private string CompanyAddress = "";
		public string companyAddress
		{
			get
			{
				return CompanyAddress;
			}
			set
			{
				if (value == null)
					CompanyAddress = "";
				else
					CompanyAddress = value;
				OnPropertyChanged(new PropertyChangedEventArgs("companyAddress"));
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
				{
					CompanyPhoneNo = "";
				}
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


		public ObservableCollection<v_Employee> engineerList
		{
			get
			{
				return myvar.employeeList;
			}
		}

		private v_Employee SelectedEngineer;
		public v_Employee selectedEngineer
		{
			get
			{
				return SelectedEngineer;
			}
			set
			{
				SelectedEngineer= value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedEngineer"));
			}
		}


		private Renamer renamer;

		private bool validate()
		{
			bool haveError = false;
			debug="";
			if (contractorList.Count==0)
			{
				debug+="Nobody in this group.\n";
				haveError=true;
			}

			if (!Validater.pass(1, AgentIdentifierID))
			{
				debug+="invalid agent citizen id\n";
				haveError=true;
			}
			if (AgentName == "" || AgentName==null)
			{
				debug += "please fill agent name\n";
				haveError = true;
			}
			if (AgentSName == "" || agentSName==null)
			{
				debug += "please fill agent surname\n";
				haveError = true;
			}
			if (AgentPhone == "")
			{
				debug+="please fill agent phone number\n";
				haveError=true;
			}
			if (SelectedCompany==null || (SelectedCompany.CompanyID==-1 && OtherCompany==""))
			{
				debug+="please choose company\n";
				haveError=true;
			}
			if (CompanyPhoneNo == "" || CompanyPhoneNo == null)
			{
				debug += "please fill company phone number\n";
				haveError = true;
			}
			if (CompanyAddress=="" || CompanyAddress==null)
			{
				debug+="please fill company address\n";
				haveError=true;
			}
			if (EmergencyPerson=="")
			{	
				debug+="please fill emergency person\n";
				haveError=true;
			}
			if (EmergencyPhone=="")
			{
				debug+="please fill emergency phone number\n";
				haveError=true;
			}
			if (SelectedEngineer==null)
			{
				debug+="please select an engineer\n";
				haveError=true;;
			}
			if (WorkingZone=="")
			{
				debug+="please fill working zone\n";
				haveError=true;
			}
			if (WorkingType=="")
			{
				debug+="please fill working type\n";
				haveError=true;
			}
			foreach(Contractor man in contractorList)
			{
				if(!man.validate())
				{
					haveError=true;
				}
			}

			if (haveError)
			{
				//rearrange the order of temporaryList
				//bring red to the front(sort by color)
				List<Contractor> tmpList = contractorList.ToList<Contractor>();
				tmpList.Sort((a, b) => string.Compare(a.borderColor, b.borderColor));
				contractorList = new ObservableCollection<Contractor>(tmpList);

			}

			//check for the same id
			foreach (Contractor manA in contractorList)
			{
				foreach (Contractor manB in contractorList)
				{
					if ((manA != manB) 
						&&
						Validater.sameOutsider(manA.identifierID,manA.identifierTypeID,manB.identifierID,manB.identifierTypeID)
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

			return !haveError;
		}

		public void submit()
		{
			if (this.validate())
			{
				//first rename temp image to a make sense name
				// real_identifierTypeID_identifierID_(Datetime replace with "_")
				renamer = new Renamer();
				foreach (Contractor person in contractorList)
				{
					if (person.imageSource != null)
					{
						string oldname = person.imageSource.UriSource.ToString().Split('/').Last();
						if (oldname.StartsWith("temp") && person.newFileName=="")		//rename temp image only
						{
							person.newFileName = "real_" + person.identifierTypeID.ToString() + "_" + person.identifierID + "(" + DateTime.Now.ToString().Replace('/', '_').Replace(':', '_').Replace(' ', '_') + ").jpg";
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

			debug += "before makeChange";
			  //////////////////////////////////////////////////////////
			 /////////////operate with database here///////////////////
			//////////////////////////////////////////////////////////

			//each Contractor makeChange about identifier,identifierImage,outsider
			Outsider agent=null;
			foreach (Contractor man in contractorList)
			{
				man.makeChange();
				if (man.identifier.IdentifierID == AgentIdentifierID)
				{
					agent = man.identifier.Outsider;
				}
			}
			if (agent == null)
			{
				debug += "Agent must be in one of contractors.";
				myvar.domainContext.RejectChanges();
				return;
			}
			else
			{
				agent.Name = AgentName;
				agent.SName = AgentSName;
				Company com;	//use for add in Coming
				if (isOtherCompanySelected)		//must pass validation
				{
					com = myvar.domainContext.Companies.FirstOrDefault(item => item.Name == this.OtherCompany || item.TelephoneNo == this.CompanyPhoneNo);
					if (com == null)
					{
						com = new Company();
						com.Name = this.OtherCompany;
						com.TelephoneNo = this.CompanyPhoneNo;
					}
					com.Address = this.CompanyAddress;	//address can be changed
				}
				else
				{
					com = this.SelectedCompany;
					com.Address = this.CompanyAddress;	//address can be changed
				}
				
				Group group;
				if (IsNewGroup)
				{
					group = new Group();
				}
				else
				{
					group = this.editGroup;
				}
				//edit attributes of this group
				group.Company = com;
				//group.CompanyAddress = companyAddress;
				group.WorkArea = WorkingZone;
				group.WorkType = WorkingType;
				group.Outsider = agent;
				group.AgentTelephoneNo = AgentPhone;
				group.EmergencyContact = EmergencyPerson;
				group.EmergencyCallNo = EmergencyPhone;
				group.EngineerEMPID = SelectedEngineer.EMPID;
				group.TimeIn = this.createTime;

				if (IsNewGroup)
				{
					myvar.domainContext.Groups.Add(group);
				}

				//last is adding or editing Outsider_Group

				foreach (Contractor man in contractorList)
				{
					Outsider_Group x;
					if (IsNewGroup)
					{
						//adding
						x = new Outsider_Group();
						x.havePhoto = man.havePhoto;
						x.isPassed = man.isPassed;
						x.Outsider = man.identifier.Outsider;
						x.Group = group;
						myvar.domainContext.Outsider_Groups.Add(x);
					}
					else
					{
						//editing
						x = myvar.domainContext.Outsider_Groups.FirstOrDefault<Outsider_Group>(item => item.Outsider == man.identifier.Outsider && item.Group == group);
						x.havePhoto = man.havePhoto;
						x.isPassed = man.isPassed;
					}
					
				}

				if (IsNewGroup)	//auto add in comming like visitor card
				{
					foreach (Contractor man in contractorList)
					{
						Coming ma = new Coming();
						ma.Outsider = man.identifier.Outsider;
						ma.TimeIn = this.createTime;
						//ma.PlateNo = this.PlateNo;
						//ma.IsDeliver = this.IsDeliver;
						ma.IsKickedOut = false;
						ma.Inspector = User.username;
						ma.ComputerName = User.computername;
						
						ma.Company = com;
						myvar.domainContext.Comings.Add(ma);
					}
				}
				debug += "before submit";
				myvar.domainContext.SubmitChanges(this.submitContractorPage_Completed, null);
			}

			myvar.busyBinding.update();
		}

		void submitContractorPage_Completed(SubmitOperation so)
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

		private string AgentIdentifierID="";
		public string agentIdentifierID
		{
			get
			{
				return AgentIdentifierID;
			}
			set
			{
				AgentIdentifierID = value;
				OnPropertyChanged(new PropertyChangedEventArgs("agentIdentifierID"));
			}
		}
		private string AgentName = "";
		public string agentName
		{
			get
			{
				return AgentName;
			}
			set
			{
				AgentName = value;
				OnPropertyChanged(new PropertyChangedEventArgs("agentName"));
			}
		}
		private string AgentSName = "";
		public string agentSName
		{
			get
			{
				return AgentSName;
			}
			set
			{
				AgentSName = value;
				OnPropertyChanged(new PropertyChangedEventArgs("agentSName"));
			}
		}
		private string AgentPhone="";
		public string agentPhone
		{
			get
			{
				return AgentPhone;
			}
			set
			{
				AgentPhone = value;
				OnPropertyChanged(new PropertyChangedEventArgs("agentPhone"));
			}
		}
		

		private string EmergencyPerson="";
		public string emergencyPerson
		{
			get
			{
				return EmergencyPerson;
			}
			set
			{
				EmergencyPerson = value;
				OnPropertyChanged(new PropertyChangedEventArgs("emergencyPerson"));
			}
		}
		private string EmergencyPhone="";
		public string emergencyPhone
		{
			get
			{
				return EmergencyPhone;
			}
			set
			{
				EmergencyPhone = value;
				OnPropertyChanged(new PropertyChangedEventArgs("emergencyPhone"));
			}
		}
		private string WorkingZone="";
		public string workingZone
		{
			get
			{
				return WorkingZone;
			}
			set
			{
				WorkingZone = value;
				OnPropertyChanged(new PropertyChangedEventArgs("workingZone"));
			}
		}
		private string WorkingType="";
		public string workingType
		{
			get
			{
				return WorkingType;
			}
			set
			{
				WorkingType = value;
				OnPropertyChanged(new PropertyChangedEventArgs("workingType"));
			}
		}
		private bool IsNewGroup;
		public bool isNewGroup
		{
			get
			{
				return IsNewGroup;
			}
			set
			{
				IsNewGroup = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isNewGroup"));
			}
		}
	}

	public class Contractor : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
		//constructor for lode to editing
		public Contractor(string _identifierID, IdentifierType _identifierType, bool _haveCopy, IdentifierImage _imageSource, bool _havePhoto, bool _isPass, RegisContractorPage _parent)
		{
			this.parent = _parent;	//this line must be at the first order

			this.identifierType = _identifierType;
			this.identifierID = _identifierID;
			
			this.haveCopy = _haveCopy;

			if (_imageSource != null)
			{
				this.imageSource = new BitmapImage(new Uri(App.Current.Host.Source, "../images/" + _imageSource.FileName));
			}
			this.havePhoto = _havePhoto;
			this.isPassed = _isPass;
			
			this.canEditID = false;
		}

		//constructor for other type of identifier(do not have image)
		public Contractor(string _identifierID, IdentifierType _identifierType, bool _haveCopy, RegisContractorPage _parent)
		{
			this.parent = _parent;	//this line must be at the first order

			this.identifierType = _identifierType;
			this.identifierID = _identifierID;

			this.haveCopy = _haveCopy;

			this.canEditID = false;

			this.havePhoto = true;
			this.isPassed = false;
		}

		//constructor for ThaiCitizenCard
		public Contractor(string _identifierID, BitmapImage _imageSource, RegisContractorPage _parent)
		{
			this.parent = _parent;	//this line must be at the first order
			this.identifierType = myvar.identifierTypeList[0];	//it's ThaiCitizenCard

			this.identifierID = _identifierID;
			this.imageSource = _imageSource;
			

			this.haveCopy = true; //it's image

			this.canEditID = true;

			this.havePhoto = true;
			this.isPassed = false;
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

		public RegisContractorPage parent { get; set; }

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

		private bool HavePhoto;
		public bool havePhoto
		{
			get
			{
				return HavePhoto;
			}
			set
			{
				HavePhoto = value;
				OnPropertyChanged(new PropertyChangedEventArgs("havePhoto"));
			}
		}

		private bool IsPassed;
		public bool isPassed
		{
			get
			{
				return IsPassed;
			}
			set
			{
				IsPassed = value;
				OnPropertyChanged(new PropertyChangedEventArgs("isPassed"));
			}
		}

		public string newFileName = "";

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
				this.borderColor = "Red";
				return false;
			}
			this.borderColor="Silver";

			return true;
		}
		
		public Identifier identifier;

		public void makeChange()
		{
			Identifier iden = myvar.domainContext.Identifiers.FirstOrDefault(item => item.IdentifierID == this.IdentifierID && item.IdentifierType == this.identifierType); ;

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

			this.identifier = iden;
			
			if (this.ImageSource != null && this.newFileName != "")
			{
				IdentifierImage img = new IdentifierImage();
				img.Identifier = iden;
				img.FileName = this.newFileName;
				img.SnapDateTime = this.parent.createTime;
				img.IsCropped = true;

				myvar.domainContext.IdentifierImages.Add(img);
			}
		}

		public bool isMatched(string searcher)
		{
			if (identifierID.Contains(searcher))
				return true;
			return false;
		}
	}
}
