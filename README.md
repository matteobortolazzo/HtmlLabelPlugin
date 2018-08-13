# HTML Label Plugin for Xamarin.Forms
Use this Xamarin.Forms plugin to display HTML content into a label.

** **MID YEAR UPDATE** **

* Xamarin.Forms 3.0
* IsHtml and RemoveHtmlTags properties removed
* HtmlAgilityPack dependency removed
* Navigating and Navitated events added when users tap on links
* Namespace and assembly name changed

## Setup
* Available on NuGet: https://www.nuget.org/packages/Xam.Plugin.HtmlLabel ![](https://img.shields.io/badge/nuget-v3.0.1-blue.svg) ![](https://matteobortolazzo.visualstudio.com/_apis/public/build/definitions/35196e9f-8b5a-4efb-af02-71d7a588c1fc/4/badge)
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

For underlined text use the &lt;u&gt; tag:
`<u>Some underlined text</u>`

**For links**: remember to add the schema (http:// https:// tel:// mailto:// ext...)

## Supported Properties
* Text
* FontAttributes
* FontFamily
* FontSize
* TextColor
* HorizontalTextAlignment

## Events
Navigating
Navigated

## Custom Properties
* MaxLines (int)


## Usage XAML

```xaml
xmlns:htmlLabel="clr-namespace:LabelHtml.Forms.Plugin.Abstractions;assembly=HtmlLabel.Forms.Plugin"
<htmlLabel:HtmlLabel Text="{Binding HtmlString}"/>
```

```xaml
xmlns:htmlLabel="clr-namespace:LabelHtml.Forms.Plugin.Abstractions;assembly=HtmlLabel.Forms.Plugin"
<htmlLabel:HtmlLabel Text="{Binding HtmlString}" htmlLabel:HtmlLabel.MaxLines="2"/>
```

## Usage C#

```csharp
var label = new HtmlLabel();
label.Text = "..htmlstring.."
HtmlLabel.SetMaxLines(label, 3);
```

## Limitations

* SetMaxLines and LineBreakMode.TailTruncation do not work properly;
* Using Links and custom fonts together could not work properly on iOS.
* Images won't be displayed on Android (TextView limitation).


### Contributions
Contributions are welcome!

### License
Under MIT, see LICENSE file.
