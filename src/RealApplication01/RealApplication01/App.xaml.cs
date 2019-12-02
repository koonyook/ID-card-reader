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

namespace RealApplication01
{
    public partial class App : Application
    {
        public App()
        {
            this.Startup += this.Application_Startup;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
			if (e.InitParams == null || e.InitParams.Count==0)
			{
				this.RootVisual = new Views.ErrorPage();
				return;
			}
			try
			{
				User.setNewUser(e.InitParams);

				//check permission of this user for open application
				if (User.permission == User.blockedPermissionName)
				{
					this.RootVisual = new Views.ErrorPage();
					return;
				}

				this.RootVisual = new MainPage();
			}
			catch (Exception error)
			{
				this.RootVisual = new Views.ErrorPage(error.Message.ToString());
				return;
			}
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
                errorWin.Show();
            }
        }
    }
}