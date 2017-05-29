## Html Label Plugin for Xamarin.Forms

Use this label on Xamarin.Forms for display HTML content.
It works on Android and iOS, on UWP it converters HTML to plain text.

### Setup
* Available on NuGet: https://www.nuget.org/packages/Xam.Plugin.HtmlLabel
* Install into your PCL project and Client projects.

**Platform Support**

|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Yes|iOS 7+|
|Xamarin.Android|Yes|API 10+|
|Windows Phone Silverlight|No||
|Windows Phone RT|No||
|Windows Store RT|No||
|Windows 10 UWP|Yes|10+|
|Xamarin.Mac|No||


### Usage XAML

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

### Usage C#

```csharp
var label = new HtmlLabel();
label.Text = "..htmlstring.."
HtmlLabel.SetMaxLines(label, 3);
```

#### Contributions
Contributions are welcome!

#### License
Under MIT, see LICENSE file.
