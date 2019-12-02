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

namespace RealApplication01.Views
{
	public partial class ErrorPage : Page
	{
		public ErrorPage()
		{
			InitializeComponent();
		}

		public ErrorPage(string errorText)
		{
			InitializeComponent();
			detailTextBlock.Text = errorText;
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

	}
}
