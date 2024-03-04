using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyDemoProject.BitmapOptimization.Views;
using MyDemoProject.ExampleDummy.Views;

namespace MyDemoProject
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static BitmapOptimizationPage BitmapOptimizationPage;
		public static ExampleDummyPage1 ExampleDummyPage1;
		public static ExampleDummyPage2 ExampleDummyPage2;

		public App()
		{
			InitializeComponent();
			BitmapOptimizationPage = new BitmapOptimizationPage();
			ExampleDummyPage1 = new ExampleDummyPage1();
			ExampleDummyPage2 = new ExampleDummyPage2();
		}

	}
}
