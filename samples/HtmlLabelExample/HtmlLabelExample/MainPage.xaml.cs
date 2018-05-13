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
		    HtmlLabeTest.Text = "<div>" +
		                            "<div>" +
		                                "<ul>" +
		                                    "<li>Something</li>" +
		                                    "<li>Something</li>" +
		                                    "<li>Something</li>" +
		                                "</ul>" +
		                            "</div>" +
                                    "<div>" +
                                        "<ol>" +
                                            "<li>Something 1</li>" +
                                            "<li>Something 2</li>" +
                                            "<li>Something 3</li>" +
                                        "</ol>" +
                                    "</div>" +
                                    "<a href=\"https://google.it\">Google</a>" +
                                "</div>";

            HtmlLabeTest.Navigated += HtmlLabeTest_Navigated;
            HtmlLabeTest.Navigating += HtmlLabeTest_Navigating;
		}

        private void HtmlLabeTest_Navigating(object sender, WebNavigatingEventArgs e)
        {
            e.Cancel = true;
        }

        private void HtmlLabeTest_Navigated(object sender, WebNavigatingEventArgs e)
        {
        }
    }
}
