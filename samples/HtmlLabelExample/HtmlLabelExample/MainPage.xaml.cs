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
		    HtmlLabeTest.Text = "<div><a href=\"http://www.google.it\">Click</a> to <strong>search</strong></div>";
		}
	}
}
