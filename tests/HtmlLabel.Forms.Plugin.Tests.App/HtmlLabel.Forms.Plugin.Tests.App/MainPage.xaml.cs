using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Colors = System.Drawing.Color;

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
        public string AlignCenter => HtmlSources.AlignCenter;
        public string AlignEnd => HtmlSources.AlignEnd;
        public string Links => HtmlSources.Links;
        public string LinksWithOptions => HtmlSources.LinksWithOptions;
        public string LinkToEmail => HtmlSources.LinkToEmail;
        public string LinkToTel => HtmlSources.LinkToTel;
        public string LinkToSms => HtmlSources.LinkToSms;
        public string LinkWithColor => HtmlSources.LinkWithColor;
        public string LinkWithoutUnderline => HtmlSources.LinkWithoutUnderline;
        public string LinkWithGestures => HtmlSources.LinkWithGestures;
        public string Arab => HtmlSources.Arab;
        public Command Clicked => new Command(() => Browser.OpenAsync("https://github.com/matteobortolazzo/HtmlLabelPlugin"));
        public BrowserLaunchOptions BrowserLaunchOptions => new BrowserLaunchOptions
        {
            LaunchMode = BrowserLaunchMode.SystemPreferred,
            TitleMode = BrowserTitleMode.Show,
            PreferredToolbarColor = Colors.AliceBlue,
            PreferredControlColor = Colors.Violet
        };
    }
}
