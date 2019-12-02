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
	public class TradeTemporaryPage : INotifyPropertyChanged
	{
		public static long selectedGroupID;
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

		public TemporaryPage parent;

	
		public TradeTemporaryPage(TemporaryPage _parent)
		{
			//must load everything before call constructor
			parent = _parent;
			if (TradeTemporaryPage.selectedGroupID <= 0)	//Come from XML
			{
				temporaryList = new ObservableCollection<Temporary>();
				this.addTemporaryListFromXml(myvar.xmlString);
				createTime = DateTime.Now;
			}
			else      //data come from group
			{
				//load specific query for getting a Group
				//include with Outsider_Group
				myvar.domainContext.Load(
					myvar.domainContext.GetGroupsWithOutsider_GroupsQuery(TradeTemporaryPage.selectedGroupID)
					, loadGroup_Completed, null);
			}

			myvar.busyBinding.update();
		
		}

		public void loadGroup_Completed(LoadOperation<Group> lo)//(object sender, EventArgs e)
		{
			//load data to myvar.regisContractorPage
			
			Group editGroup = myvar.domainContext.Groups.FirstOrDefault<Group>(item => item.GroupID==TradeTemporaryPage.selectedGroupID);
			if (editGroup == null)
			{
				debug = "ERROR:Cannot load that group to edit";
			}
			else
			{
				//selectedCompany = editGroup.Company;	//this cannnot be null
				
				//createTime = editGroup.TimeIn; this is a bug
				createTime = DateTime.Now;

				temporaryList = new ObservableCollection<Temporary>();
				//debug = editGroup.Outsider_Group.Count().ToString();
				foreach (Outsider_Group man in editGroup.Outsider_Group)
				{
					Temporary newtem = new Temporary(man,this);
					this.temporaryList.Add(newtem);
				}
				//debug += "???";
				updateTemporaryCardID();
				OnPropertyChanged(new PropertyChangedEventArgs("temporaryList"));
				
				//debug = this.contractorList.Count().ToString();
			}
			//parent.listBox1.ItemsSource = this.contractorList;
			myvar.busyBinding.update();
		}
		
		public void updateTemporaryCardID()
		{
			bool[] cardSlot = new bool[myconstant.Number_of_Temporary_Card+1];
			for (int i = 0; i < myconstant.Number_of_Temporary_Card + 1; i++)
			{
				cardSlot[i] = false;
			}
			
			IEnumerable<Coming> comings = myvar.domainContext.Comings.Where<Coming>(
				item =>
					 //item.TimeIn.Date == DateTime.Today &&
					 item.TimeOut==null);
			
			foreach (Coming com in comings)
			{
				if(com.TemporaryCardID!=null)
					cardSlot[(int)com.TemporaryCardID] = true;
			}
			
			int runner = 1;
			bool left = true;
			foreach (Temporary card in temporaryList)
			{
				while (runner<=myconstant.Number_of_Temporary_Card && cardSlot[runner] == true)
				{
					runner++;
				}
				if (runner > myconstant.Number_of_Temporary_Card)
				{
					debug = "No Temporary Card Left.";
					left = false;
					runner++;
				}
				if (left)
				{
					card.tempCardID = (short)runner;
					runner++;
				}
				else
				{
					card.tempCardID = 0;		//for backup card
				}
			}
			OnPropertyChanged(new PropertyChangedEventArgs("concludeTemporaryCardID"));
			OnPropertyChanged(new PropertyChangedEventArgs("concludeString"));
		}

		public string concludeString
		{
			get
			{
				if (temporaryList == null)
				{
					return "Loading...";
				}
				//things that must to conclude
				int allCount = temporaryList.Count;
				int blackCount = 0;	
				int nodataCount = 0; 
				int delayCount = 0;	
				int notpassCount = 0; 
				foreach (Temporary man in temporaryList)
				{
					if (man.isBlack())
					{
						blackCount++;
					}
					int tmp = man.getRestDay();
					if (tmp > myconstant.Days_to_Expire_Temporary_Card)
					{
						nodataCount++;
					}
					else//case that have data
					{
						if (tmp < 0)
							delayCount++;

						if (!man.getIsPassed())
							notpassCount++;
					}
				}
				string ret = allCount+" People";
				if (blackCount > 0)
					ret += ", " + blackCount + " Blacklist";
				if (nodataCount > 0)
					ret += ", " + nodataCount + " No data";
				if (notpassCount > 0)
					ret += ", " + notpassCount + " Not pass test";
				if (delayCount > 0)
					ret += ", " + delayCount + " Temporary card time out";
				return ret;
			}
		}

		public string concludeTemporaryCardID
		{
			get
			{
				if (temporaryList == null)	//in case of first time
				{
					return "Loading...";
				}
				int backupCount = 0;
				List<short> use = new List<short>();
				foreach (Temporary card in temporaryList)
				{
					if (card.tempCardID == 0)
					{
						backupCount++;
					}
					else if ((card.tempCardID != 0) && (!use.Contains(card.tempCardID)))
					{
						use.Add(card.tempCardID);
					}
				}
				use.Sort();
				int start=-1, end=-1;
				string ret = "";
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
						else if (start < end)
						{

							ret += start.ToString() + "-" + end.ToString() + ", ";
						}
					}
				}
				return ret;
			}
		}
		private ObservableCollection<Temporary> TemporaryList;
		public ObservableCollection<Temporary> temporaryList 
		{ 
			get
			{
				return TemporaryList;
			}

			set
			{
				TemporaryList = value;
				OnPropertyChanged(new PropertyChangedEventArgs("temporaryList"));
			}
		}

		private Temporary SelectedTemporary=null;
		public Temporary selectedTemporary
		{
			get
			{
				return SelectedTemporary;
			}
			set
			{
				SelectedTemporary = value;
				OnPropertyChanged(new PropertyChangedEventArgs("selectedTemporary"));
			}
		}

		public void addTemporaryListFromXml(string xmlString)
		{
			XmlReader reader = XmlReader.Create(new StringReader(xmlString));

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "card")
				{
					string cardID = reader.GetAttribute("ID");
					string filename = reader.GetAttribute("filename");
					Temporary newTemp = new Temporary(
						cardID,
						new BitmapImage(new Uri(App.Current.Host.Source, "../images/" + filename)),
						this);
					this.temporaryList.Add(newTemp);
				}
			}
			updateTemporaryCardID();
		}

		
		private Renamer renamer;

		private bool validate()
		{
			bool haveError = false;
			debug="";
			if (temporaryList.Count==0)
			{
				debug+="Nobody.\n";
				haveError=true;
			}

			foreach(Temporary man in temporaryList)
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
				List<Temporary> tmpList = temporaryList.ToList<Temporary>();
				tmpList.Sort((a,b) => string.Compare(a.borderColor,b.borderColor));
				temporaryList = new ObservableCollection<Temporary>(tmpList);

			}
			//check for the same id
			foreach (Temporary manA in temporaryList)
			{
				foreach (Temporary manB in temporaryList)
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
				foreach (Temporary person in temporaryList)
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
			foreach (Temporary man in temporaryList)
			{
				man.makeChange();
			}
			
			debug += "before submit";
			myvar.domainContext.SubmitChanges(this.submitContractorPage_Completed, null);
			
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
			}//throw new NotImplementedException();
			
		}

	}

	public class Temporary : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
		
		public TradeTemporaryPage parent { get; set; }
		private Outsider_Group Link;

		//constructor for adding from group
		public Temporary(Outsider_Group _outsider_group, TradeTemporaryPage _parent)
		{
			Link = _outsider_group;
			parent = _parent;

			Identifier iden = Link.Outsider.Identifiers.Last<Identifier>();
			this.identifierType = iden.IdentifierType;
			this.IdentifierID = iden.IdentifierID;
			

			IdentifierImage idenImage = iden.IdentifierImages.LastOrDefault<IdentifierImage>();
			if(idenImage != null)
			{
				imageSource = new BitmapImage(new Uri(App.Current.Host.Source, "../images/" 
					+ idenImage.FileName));
			}
			
			this.canEditID = false;
		}

		//constructor for other type of identifier(do not have image) from new person
		public Temporary(string _identifierID, IdentifierType _identifierType, bool _haveCopy, TradeTemporaryPage _parent)
		{
			this.parent = _parent;

			//try to find Outsider_Group
			this.identifierType = _identifierType;
			this.IdentifierID = _identifierID;
			

			Identifier iden=myvar.domainContext.Identifiers.FirstOrDefault<Identifier>(
				item => Validater.sameOutsider(_identifierID,_identifierType.IdentifierTypeID, item.IdentifierID, item.IdentifierTypeID)
			);

			if(iden==null)
			{
				this.Link=null;
			}
			else
			{
				IdentifierImage idenImage = iden.IdentifierImages.LastOrDefault<IdentifierImage>();
				if(idenImage != null)
				{
					imageSource = new BitmapImage(new Uri(App.Current.Host.Source, "../images/" 
						+ idenImage.FileName));
				}
				this.Link=iden.Outsider.Outsider_Group.LastOrDefault<Outsider_Group>();
			}
			
			this.haveCopy = _haveCopy;
			this.canEditID = false;
		}

		//constructor for ThaiCitizenCards from add People 
		public Temporary(string _identifierID, BitmapImage _imageSource, TradeTemporaryPage _parent)
		{
			this.parent = _parent;

			this.identifierType = myvar.identifierTypeList[0];	//it's ThaiCitizenCard
			this.IdentifierID = _identifierID;

			this.imageSource = _imageSource;
			this.haveCopy = true; //it's image

			Identifier iden=myvar.domainContext.Identifiers.FirstOrDefault<Identifier>(
				item => Validater.sameOutsider(this.identifierID,this.identifierType.IdentifierTypeID, item.IdentifierID, item.IdentifierTypeID)
			);

			if(iden==null)
			{
				this.Link=null;
			}
			else
			{
				this.Link=iden.Outsider.Outsider_Group.LastOrDefault<Outsider_Group>();
			}

			this.canEditID = true;
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

				Identifier iden=myvar.domainContext.Identifiers.FirstOrDefault<Identifier>(
					item => Validater.sameOutsider(this.identifierID,this.identifierType.IdentifierTypeID, item.IdentifierID, item.IdentifierTypeID)
				);

				if(iden==null)
				{
					this.Link=null;
				}
				else
				{
					this.Link=iden.Outsider.Outsider_Group.LastOrDefault<Outsider_Group>();
				}

				OnPropertyChanged(new PropertyChangedEventArgs("identifierID"));
				OnPropertyChanged(new PropertyChangedEventArgs("safetyStatus"));
				OnPropertyChanged(new PropertyChangedEventArgs("testStatus"));
				OnPropertyChanged(new PropertyChangedEventArgs("restday"));
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

		public string testStatus
		{
			get
			{
				if(Link==null)
				{
					return "ไม่ได้สมัคร";
				}
				else if(Link.isPassed)
				{
					return "ผ่าน";
				}
				else
				{
					return "ไม่ผ่าน";
				}
			}
		}

		public bool getIsPassed() //false have 2 meaning(nodata or notpass)
		{
			if (Link == null)
				return false;
			else
				return Link.isPassed;
		}

		public string safetyStatus
		{
			get
			{
				if(isBlack())
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

		public string restDay
		{
			get
			{
				int tmp=getRestDay();
				if (tmp > myconstant.Days_to_Expire_Temporary_Card)
				{
					return "ไม่มีข้อมูล";
				}
				else
				{
					return tmp.ToString();
				}
			}
		}

		public int getRestDay()
		{
			if(Link==null)
			{
				return myconstant.Days_to_Expire_Temporary_Card+1;	//must remember
			}
			else
			{
				int old=(DateTime.Now - Link.Group.TimeIn).Days;
				return (myconstant.Days_to_Expire_Temporary_Card-old);
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
			if (!Validater.pass(identifierType, IdentifierID) || Link==null || Link.isPassed==false)
			{
				this.borderColor = "Red";
				return false;
			}
			this.borderColor="Silver";

			return true;
		}
		
		//public Identifier identifier;
		private short TempCardID;
		public short tempCardID
		{
			get
			{
				return TempCardID;
			}
			set
			{
				TempCardID = value;
				OnPropertyChanged(new PropertyChangedEventArgs("tempCardID"));
			}
		}

		public void makeChange()
		{
			//make change with coming only
			//must verify before call this method
			Coming newComing = new Coming();
			newComing.Outsider = Link.Outsider;
			newComing.TimeIn = parent.createTime;
			newComing.IsKickedOut = false;
			newComing.Inspector = User.username;
			newComing.ComputerName = User.computername;
			newComing.TemporaryCardID = tempCardID;
			newComing.Company = Link.Group.Company;
			myvar.domainContext.Comings.Add(newComing);

			/* do nothing about new identifier image
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
			 */
		}
	}
}
