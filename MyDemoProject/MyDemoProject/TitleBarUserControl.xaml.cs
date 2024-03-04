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
	/// Interaction logic for TitleBarUserControl.xaml
	/// </summary>
	public partial class TitleBarUserControl : UserControl
	{
		public event EventHandler MinimizeEvent;
		public event EventHandler MaximizeEvent;
		public event EventHandler CloseEvent;
		public event EventHandler MouseDownEvent;

		public TitleBarUserControl()
		{
			InitializeComponent();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			MinimizeEvent.Invoke(this, EventArgs.Empty);
		}

		private void MaximizeButton_Click(object sender, RoutedEventArgs e)
		{
			MaximizeEvent.Invoke(this, EventArgs.Empty);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			CloseEvent.Invoke(this, EventArgs.Empty);
		}

	}
}
