using MyDemoProject.BitmapOptimization.Views;
using MyDemoProject.ExampleDummy.Views;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly BitmapOptimizationPage mBitmapOptimizationPage;
		private readonly ExampleDummyPage1 mExampleDummyPage1;
		private readonly ExampleDummyPage2 mExampleDummyPage2;

		public MainWindow()
		{
			InitializeComponent();

			mBitmapOptimizationPage = App.BitmapOptimizationPage;
			mExampleDummyPage1 = App.ExampleDummyPage1;
			mExampleDummyPage2 = App.ExampleDummyPage2;

			MainFrame.Navigate(mBitmapOptimizationPage);

			NavigationBarUserControl.Button1ClickEvent += Button1Command;
			NavigationBarUserControl.Button2ClickEvent += Button2Command;
			NavigationBarUserControl.Button3ClickEvent += Button3Command;
			NavigationBarUserControl.Button4ClickEvent += Button4Command;
			TitleBarUserControl.MinimizeEvent += MinimizeButtonCommand;
			TitleBarUserControl.MaximizeEvent += MaximizeButtonCommand;
			TitleBarUserControl.CloseEvent += CloseButtonCommand;
			TitleBarUserControl.MouseDown += MouseDownCommand;
		}

		private void MouseDownCommand(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void CloseButtonCommand(object sender, EventArgs e)
		{
			Close();
		}

		private void MaximizeButtonCommand(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Maximized)
			{
				WindowState = WindowState.Normal;
			}
			else
			{
				WindowState = WindowState.Maximized;
			}
		}

		private void MinimizeButtonCommand(object sender, EventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void Button4Command(object sender, EventArgs e)
		{
			MainFrame.Navigate(mBitmapOptimizationPage);
		}

		private void Button3Command(object sender, EventArgs e)
		{
			MainFrame.Navigate(mExampleDummyPage2);
		}

		private void Button2Command(object sender, EventArgs e)
		{
			MainFrame.Navigate(mExampleDummyPage1);
		}

		private void Button1Command(object sender, EventArgs e)
		{
			MainFrame.Navigate(mBitmapOptimizationPage);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			NavigationBarUserControl.Button1ClickEvent -= Button1Command;
			NavigationBarUserControl.Button1ClickEvent -= Button2Command;
			NavigationBarUserControl.Button1ClickEvent -= Button3Command;
			NavigationBarUserControl.Button1ClickEvent -= Button4Command;

			TitleBarUserControl.MinimizeEvent -= MinimizeButtonCommand;
			TitleBarUserControl.MaximizeEvent -= MaximizeButtonCommand;
			TitleBarUserControl.CloseEvent -= CloseButtonCommand;
		}
	}
}
