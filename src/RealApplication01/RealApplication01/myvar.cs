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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RealApplication01.Web;
using RealApplication01.Views;
using System.ServiceModel.DomainServices.Client;

namespace RealApplication01
{
	public class myconstant
	{
		public const int Days_to_Expire_Temporary_Card=3;
		public const int Number_of_Temporary_Card=70;	//this will use card number 1 to 70
	}

	public class myvar
	{
		public static string serializedImage;

		public static string xmlString;

		public static DomainService1 domainContext 
			= new DomainService1();

		public static BusyBinding busyBinding
			= new BusyBinding();

		//for debugging
		public static Page currentPage;

		//bind with visitor page
		public static CheckInVisitorPage checkInVisitorPage;
		public static RegisContractorPage regisContractorPage;
		public static TradeTemporaryPage tradeTemporaryPage;
		public static EveryCheckOutPage everyCheckOutPage;

		public static ObservableCollection<Company> companyList;		
		//public static LoadOperation<Company> loadCompany;

		public static ObservableCollection<v_Section> sectionList;
		//public static LoadOperation<v_Section> loadSection;

		public static ObservableCollection<v_Employee> employeeList;
		//public static LoadOperation<v_Employee> loadEmployee;

		public static ObservableCollection<IdentifierType> identifierTypeList;
		//public static LoadOperation<IdentifierType> loadIdentifierType;

		public static ObservableCollection<v_BlackIdentifierList> blackIdentifierList;
		//public static LoadOperation<v_BlackIdentifierList> loadBlackIdentifierList;

		public static void loadCompany_Completed(LoadOperation<Company> lo)//(object sender, EventArgs e)
		{
			getNewCompanyList();
			myvar.busyBinding.update();
		}

		public static void getNewCompanyList()
		{
			myvar.companyList = new ObservableCollection<Company>(myvar.domainContext.Companies);

			Company other = new Company();
			other.CompanyID = -1;
			other.Name = "Other...";
			myvar.companyList.Add(other);
		}

		public static void loadSection_Completed(LoadOperation<v_Section> lo)
		{
			myvar.sectionList = new ObservableCollection<v_Section>(lo.Entities);
			//myvar.loadSection.Completed -= loadSection_Completed;
			//throw new NotImplementedException();
			myvar.busyBinding.update();
		}

		public static void loadEmployee_Completed(LoadOperation<v_Employee> lo)
		{
			myvar.employeeList = new ObservableCollection<v_Employee>(lo.Entities);
			//myvar.loadEmployee.Completed -= loadEmployee_Completed;
			//throw new NotImplementedException();
			myvar.busyBinding.update();
		}

		public static void loadIdentifierType_Completed(LoadOperation<IdentifierType> lo)
		{
			myvar.identifierTypeList = new ObservableCollection<IdentifierType>(lo.Entities);
			//myvar.loadIdentifierType.Completed -= loadIdentifierType_Completed;
			//throw new NotImplementedException();
			myvar.busyBinding.update();
		}

		public static void loadBlackIdentifier_Completed(LoadOperation<v_BlackIdentifierList> lo)
		{
			myvar.blackIdentifierList = new ObservableCollection<v_BlackIdentifierList>(lo.Entities);
			//myvar.loadBlackIdentifierList.Completed -= loadBlackIdentifier_Completed;
			//throw new NotImplementedException();
			myvar.busyBinding.update();
		}

		//this set for VisitorPage
		public static LoadOperation<Identifier> loadIdentifier;
		public static LoadOperation<Outsider> loadOutsider;
		
		public static void updateBusyBinding(object sender, EventArgs e)
		{
			myvar.busyBinding.update();
			//myvar.loadBlackIdentifierList.Completed -= loadSomething_Completed;
			//throw new NotImplementedException();
		}

	}
}
