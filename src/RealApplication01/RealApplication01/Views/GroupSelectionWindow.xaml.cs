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

namespace RealApplication01.Views
{
	public partial class GroupSelectionWindow : ChildWindow
	{
		public long answerGroupID;
		public GroupSelectionWindow()
		{
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (v_GroupForSelectDataGrid.SelectedIndex == -1)
			{
				MessageBox.Show("Please select a group");
			}
			else
			{
				answerGroupID = (v_GroupForSelectDataGrid.SelectedItem as v_GroupForSelect).GroupID;
				this.DialogResult = true;
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void groupDomainDataSource_LoadedData(object sender, System.Windows.Controls.LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//myvar.domainContext.Load<Group>(myvar.domainContext.GetGroupsQuery());
			//groupDataGrid.ItemsSource = myvar.domainContext.Groups;
		}

		private void v_GroupForSelectDomainDataSource_LoadedData(object sender, LoadedDataEventArgs e)
		{

			if (e.HasError)
			{
				System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
				e.MarkErrorAsHandled();
			}
		}

		private void searcherTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				outsiderDomainDataSourceLoadButton.Focus();
				v_GroupForSelectDomainDataSource.Load();
			}
		}
	}
}

