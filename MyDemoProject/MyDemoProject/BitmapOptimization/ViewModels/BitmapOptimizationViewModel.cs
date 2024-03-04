using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonUtilities;
using MyDemoProject.ExampleDummy.Views;

namespace MyDemoProject.BitmapOptimization.ViewModels
{
	public class BitmapOptimizationViewModel
	{
		public BitmapOptimizationViewModel()
		{

		}

		internal void Page_Unloaded()
		{
			AutoClosingWindow auto = new AutoClosingWindow("Have you Fucking Enjoyed it?!!", 1000);
			auto.Show();
		}
	}
}
