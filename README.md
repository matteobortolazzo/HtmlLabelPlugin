[![Build Status](https://dev.azure.com/matteobortolazzo/HtmlLabel/_apis/build/status/matteobortolazzo.HtmlLabelPlugin?branchName=master)](https://dev.azure.com/matteobortolazzo/HtmlLabel/_build/latest?definitionId=26&branchName=master)
[![Downloads](https://img.shields.io/nuget/dt/Xam.Plugin.HtmlLabel.svg)](https://www.nuget.org/packages/Xam.Plugin.HtmlLabel/)

# HTML Label Plugin for Xamarin.Forms
Use this Xamarin.Forms plugin to display HTML content into a label.

## Setup
* Available on [NuGet](https://www.nuget.org/packages/Xam.Plugin.HtmlLabel)
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

**WARNING**: not all styles are supported by each platform!

## Supported Properties
* Text
* FontAttributes
* FontFamily
* FontSize
* TextColor
* HorizontalTextAlignment
* UnderlineText
* LinkColor
* BrowserLaunchOptions
* AndroidLegacyMode

## Events
Navigating
Navigated

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
```

## Links
It is possible to customize *links*: 
* Color
* Underline
* Browser options

[*Browser options*](https://docs.microsoft.com/en-us/xamarin/essentials/open-browser) are applied with the following schemas: *http*, *https*, *mailto*, *tel*, *sms* and *geo*.

**iOS WARNING**: Changing the style of iOS links is not supported by *iOS* intentionally and this plugin use a workaround. From *Apple*:
> To promote consistency, the intended behavior is for ranges that represent links (specified via NSLinkAttributeName) to be drawn using the default link appearance.

## Android's Legacy Mode
Settings the property `AndroidLegacyMode` to *True* the renderer separates block-level elements with blank lines.

## Limitations

* XF Label's TextType="HTML" disables this implementation.
* Images are not displayed on Android (TextView limitation).
* Adding *GestureRecognizers* will disabled links.

### Contributions
Contributions are welcome!

### License
Under MIT, see LICENSE file.
