﻿namespace HyperTextLabel.Maui.Tests.App;

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
    public string List => HtmlSources.List;
    public string AlignCenter => HtmlSources.AlignCenter;
    public string AlignEnd => HtmlSources.AlignEnd;
    public string Links => HtmlSources.Links;
    public string LinksWithOptions => HtmlSources.LinksWithOptions;
    public string LinkToEmail => HtmlSources.LinkToEmail;
    public string LinkToTel => HtmlSources.LinkToTel;
    public string LinkToTelAlternative => HtmlSources.LinkToTelAlternate;
    public string LinkToSms => HtmlSources.LinkToSms;
    public string LinkToGeo => HtmlSources.LinkToGeo;
    public string LinkWithColor => HtmlSources.LinkWithColor;
    public string LinkWithoutUnderline => HtmlSources.LinkWithoutUnderline;
    public string LinkWithGestures => HtmlSources.LinkWithGestures;
    public string CustomFont => HtmlSources.CustomFont;
    public string Arab => HtmlSources.Arab;
    public string Image => HtmlSources.Image;
    public string Paragraphs => HtmlSources.Paragraphs;
    public string LineHeight => HtmlSources.LineHeight;
    public Command Clicked => new Command(() => Browser.OpenAsync("https://github.com/matteobortolazzo/HtmlLabelPlugin"));
    public BrowserLaunchOptions BrowserLaunchOptions => new BrowserLaunchOptions
    {
        LaunchMode = BrowserLaunchMode.SystemPreferred,
        TitleMode = BrowserTitleMode.Show,
        PreferredToolbarColor = Colors.AliceBlue,
        PreferredControlColor = Colors.Violet
    };
}