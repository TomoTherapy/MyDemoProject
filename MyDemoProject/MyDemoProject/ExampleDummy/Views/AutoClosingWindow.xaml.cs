using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyDemoProject.ExampleDummy.Views
{
	/// <summary>
	/// Interaction logic for AutoClosingWindow.xaml
	/// </summary>
	public partial class AutoClosingWindow : Window
	{
		public AutoClosingWindow(string message, int milliseconds)
		{
			InitializeComponent();

			MessageBlock.Text = message;

			Task task = Task.Run(() =>
			{
				Thread.Sleep(milliseconds);
				Dispatcher.Invoke(new Action(() =>
				{
					Close();
				}));
			});
		}
	}
}
