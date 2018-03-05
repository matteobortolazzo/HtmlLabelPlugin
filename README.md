# Html Label Plugin for Xamarin.Forms
Use this Xamarin.Forms plugin to display HTML content into a label.

** **NEW YEAR UPDATE** **

* Xamarin.Forms 2.5.0;
* .NET Standard;
* UWP partial support;
* Added support for: FontAttributes, FontFamily, FontSize, TextColor and HorizontalTextAlignment;
* HyperLinks support.

## Setup
* Available on NuGet: https://www.nuget.org/packages/Xam.Plugin.HtmlLabel
* Install it in every Xamarin.Forms project.
* **iOS**: AppDelegate.cs
    ```cs
    HtmlLabelRenderer.Initialize();
    global::Xamarin.Forms.Forms.Init();
    ```
* **Android**: MainActivity.cs
    ```cs
    HtmlLabelRenderer.Initialize();
    global::Xamarin.Forms.Forms.Init(this, bundle);
    ```
* **UWP**: App.xaml.cs
    ```cs
    var rendererAssemblies = new[] { typeof(HtmlLabelRenderer).GetTypeInfo().Assembly };
    Xamarin.Forms.Forms.Init(e, rendererAssemblies);
    HtmlLabelRenderer.Initialize();
    ```      

## How it works
On iOS and Android it uses the native HTML rendering capabilities of iOS's UILabel and Android's TextView. 

UWP's TextBlock cannot renders HTML so the library parses the HTML and uses Inlines to display: `<a>, <b>, <br>, <em>, <i>, <p>, <strong>, <u>, <ul> <li>, <div>`.

FontAttributes, FontFamily, FontSize, TextColor, HorizontalTextAlignment are converted into inline CSS in a wrapping `<div>` for iOS and Android. UWP supports them natively.

## Custom styling
If you need to customize something in Android or iOS you can use inline CSS, for example: 

`<span style="color: green">...</span>`

For underlined text use the <u> tag:
`<u>Some underlined text</u>`

**For links**: remember to add the schema (http:// https:// tel:// mailto:// ext...)

## Supported Properties
* Text
* FontAttributes
* FontFamily
* FontSize
* TextColor
* HorizontalTextAlignment


## Custom Properties
* MaxLines (int)
* IsHtml (bool)
* RemoveHtmlTags (bool)


## Usage XAML

```xaml
xmlns:htmlLabel="clr-namespace:Plugin.HtmlLabel;assembly=Plugin.HtmlLabel"
<htmlLabel:HtmlLabel Text="{Binding HtmlString}"/>
```

```xaml
xmlns:htmlLabel="clr-namespace:Plugin.HtmlLabel;assembly=Plugin.HtmlLabel"
<htmlLabel:HtmlLabel Text="{Binding HtmlString}" htmlLabel:HtmlLabel.MaxLines="2"/>
```

```xaml
xmlns:htmlLabel="clr-namespace:Plugin.HtmlLabel;assembly=Plugin.HtmlLabel"
<htmlLabel:HtmlLabel Text="{Binding PlainTextString}" htmlLabel:HtmlLabel.IsHtml="False"/>
```

```xaml
xmlns:htmlLabel="clr-namespace:Plugin.HtmlLabel;assembly=Plugin.HtmlLabel"
<htmlLabel:HtmlLabel Text="{Binding HtmlString}" htmlLabel:HtmlLabel.RemoveHtmlTags="True"/>
```

## Usage C#

```csharp
var label = new HtmlLabel();
label.Text = "..htmlstring.."
HtmlLabel.SetMaxLines(label, 3);
```

### Contributions
Contributions are welcome!

### License
Under MIT, see LICENSE file.
