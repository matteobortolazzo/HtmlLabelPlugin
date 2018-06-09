using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestHtmlLabel
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			HomeLabel.Text = "<strong>Hello</strong> world!";
		}
	}
}
