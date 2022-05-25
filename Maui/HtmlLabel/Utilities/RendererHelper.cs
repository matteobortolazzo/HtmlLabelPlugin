using HyperTextLabel.Maui.Controls;
using HyperTextLabel.Maui.Extensions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("HyperTextLabel.Maui.Shared.Tests")]
namespace HyperTextLabel.Maui.Utilities
{
    internal class RendererHelper
    {
        private readonly IHtmlLabel _label;
        private readonly DevicePlatform _runtimePlatform;
        private readonly bool _isRtl;
        private readonly string _text;
        private readonly IList<KeyValuePair<string, string>> _styles;
        private static readonly string[] SupportedProperties = {
                                                      Label.TextProperty.PropertyName,
                                                      Label.TextColorProperty.PropertyName,
                                                      Label.FontAttributesProperty.PropertyName,
                                                      Label.FontFamilyProperty.PropertyName,
                                                      Label.FontSizeProperty.PropertyName,
                                                      Label.LineBreakModeProperty.PropertyName,
                                                      Label.HorizontalTextAlignmentProperty.PropertyName,
                                                      Label.LineHeightProperty.PropertyName,
                                                      Label.PaddingProperty.PropertyName,
                                                      HtmlLabel.LinkColorProperty.PropertyName
        };

        public RendererHelper(IHtmlLabel label, string text, DevicePlatform runtimePlatform, bool isRtl)
        {
            _label = label ?? throw new ArgumentNullException(nameof(label));
            _runtimePlatform = runtimePlatform;
            _isRtl = isRtl;
            _text = text?.Trim();
            _styles = new List<KeyValuePair<string, string>>();
        }

        public void AddFontAttributesStyle(FontAttributes fontAttributes)
        {
            if (fontAttributes == FontAttributes.Bold)
            {
                AddStyle("font-weight", "bold");
            }
            else if (fontAttributes == FontAttributes.Italic)
            {
                AddStyle("font-style", "italic");
            }
        }

        public void AddFontFamilyStyle(string fontFamily)
        {
            string GetSystemFont() => _runtimePlatform == DevicePlatform.iOS ? "-apple-system" :
                _runtimePlatform == DevicePlatform.Android ? "Roboto" :
                _runtimePlatform == DevicePlatform.WinUI ? "Segoe UI" :
                "system-ui";

            var fontFamilyValue = string.IsNullOrWhiteSpace(fontFamily)
                 ? GetSystemFont()
                 : fontFamily;
            AddStyle("font-family", $"'{fontFamilyValue}'");
        }

        public void AddFontSizeStyle(double fontSize)
        {
            AddStyle("font-size", $"{fontSize}px");
        }

        public void AddTextColorStyle(Color color)
        {
            if (color.IsDefault())
            {
                return;
            }

            var red = (int)(color.Red * 255);
            var green = (int)(color.Green * 255);
            var blue = (int)(color.Blue * 255);
            var alpha = color.Alpha;
            var hex = $"#{red:X2}{green:X2}{blue:X2}";
            var rgba = $"rgba({red},{green},{blue},{alpha})";
            AddStyle("color", hex);
            AddStyle("color", rgba);
        }

        public void AddHorizontalTextAlignStyle(TextAlignment textAlignment)
        {
            if (textAlignment == TextAlignment.Start)
            {
                AddStyle("text-align", _isRtl ? "right" : "left");
            }
            else if (textAlignment == TextAlignment.Center)
            {
                AddStyle("text-align", "center");
            }
            else if (textAlignment == TextAlignment.End)
            {
                AddStyle("text-align", _isRtl ? "left" : "right");
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_text))
            {
                return null;
            }

            AddFontAttributesStyle(_label.FontAttributes);
            AddFontFamilyStyle(_label.FontFamily);
            AddTextColorStyle(_label.TextColor);
            AddHorizontalTextAlignStyle(_label.HorizontalTextAlignment);
            AddFontSizeStyle(_label.FontSize);

            var style = GetStyle();
            return $"<div style=\"{style}\" dir=\"auto\">{_text}</div>";
        }

        public string GetStyle()
        {
            var builder = new StringBuilder();

            foreach (KeyValuePair<string, string> style in _styles)
            {
                _ = builder.Append($"{style.Key}:{style.Value};");
            }

            var css = builder.ToString();
            if (_styles.Any())
            {
                css = css.Substring(0, css.Length - 1);
            }

            return css;
        }

        public static bool RequireProcess(string propertyName) => SupportedProperties.Contains(propertyName);

        /// <summary>
        /// Handles the Uri for the following types:
        /// - Web url
        /// - Email
        /// - Telephone
        /// - SMS
        /// - GEO
        /// </summary>
        /// <param name="label"></param>
        /// <param name="url"></param>
        /// <returns>true if the uri has been handled correctly, false if the uri is not handled because of an error</returns>
        public static bool HandleUriClick(IHtmlLabel label, string url)
        {

            if (url == null || !Uri.IsWellFormedUriString(WebUtility.UrlEncode(url), UriKind.RelativeOrAbsolute))
            {
                return false;
            }

            var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);

            label.SendNavigating(args);

            if (args.Cancel)
            {
                // Uri is handled because it is cancled;
                return true;
            }
            bool result = false;
            var uri = new Uri(url);

            if (uri.IsHttp())
            {
                uri.LaunchBrowser(label.BrowserLaunchOptions);
                result = true;
            }
            else if (uri.IsEmail())
            {
                result = uri.LaunchEmail();
            }
            else if (uri.IsTel())
            {
                result = uri.LaunchTel();
            }
            else if (uri.IsSms())
            {
                result = uri.LaunchSms();
            }
            else if (uri.IsGeo())
            {
                result = uri.LaunchMaps();
            }
            else
            {
                result = Launcher.TryOpenAsync(uri).Result;
            }
            // KWI-FIX What to do if the navigation failed? I assume not to spawn the SendNavigated event or introduce a fail bit on the args 
            label.SendNavigated(args);
            return result;
        }

        private void AddStyle(string selector, string value)
        {
            _styles.Add(new KeyValuePair<string, string>(selector, value));
        }
    }
}
