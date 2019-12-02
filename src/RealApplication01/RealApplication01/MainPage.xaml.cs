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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using RealApplication01.Web;
using System.Collections.ObjectModel;

namespace RealApplication01
{
    public partial class MainPage : UserControl
    {
		public MainPage()
        {
            InitializeComponent();
            
			//Binding layoutRoot for show busyIndicator in every page
			this.LayoutRoot.DataContext = myvar.busyBinding;

			//show username and computer name at the top-right of every page.
			userIdentifierTextBlock.Text = "Hello, "+User.getString();

			//preload for some collection that will be in combobox

			myvar.domainContext.Load(myvar.domainContext.GetCompaniesQuery(),myvar.loadCompany_Completed,null);

			myvar.domainContext.Load(myvar.domainContext.GetV_SectionQuery(),myvar.loadSection_Completed,null);

			myvar.domainContext.Load(myvar.domainContext.GetV_EmployeeQuery(),myvar.loadEmployee_Completed,null);

			myvar.domainContext.Load(myvar.domainContext.GetIdentifierTypesQuery(),myvar.loadIdentifierType_Completed,null);

			myvar.domainContext.Load(myvar.domainContext.GetV_BlackIdentifierListQuery(),myvar.loadBlackIdentifier_Completed,null);

			myvar.busyBinding.update();
		}

		

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }
    }
}