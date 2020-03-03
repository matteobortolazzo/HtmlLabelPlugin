using System.ComponentModel;
using Xamarin.Forms;

namespace HtmlLabel.Forms.Plugin.Tests.App
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new Sources();
        }
    }

    public class Sources
    {
        public string Bold => HtmlSources.Bold;
        public string Italic => HtmlSources.Italic;
        public string Color => HtmlSources.Color;
    }
}
