using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyDemoProject
{
	/// <summary>
	/// Interaction logic for NavigationBarUserControl.xaml
	/// </summary>
	public partial class NavigationBarUserControl : UserControl
	{
		public event EventHandler Button1ClickEvent;
		public event EventHandler Button2ClickEvent;
		public event EventHandler Button3ClickEvent;
		public event EventHandler Button4ClickEvent;

		public NavigationBarUserControl()
		{
			InitializeComponent();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Button1ClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			Button2ClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			Button3ClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			Button4ClickEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}
