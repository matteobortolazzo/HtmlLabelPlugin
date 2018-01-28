using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HtmlLabelExample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		    HtmlLabel2.Text = "<p><u>Test</u> <span style=\"color: #3366ff;\">deneme</span></p>";
		}
	}
}
