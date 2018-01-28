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
		    HtmlLabel2.Text = "<div>My first paragraph. <a href=\"http://www.google.com\">Go to Google</a></div>";
		}
	}
}
