using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("HtmlLabel.Forms.Plugin.Shared.Tests")]
namespace LabelHtml.Forms.Plugin.Abstractions
{
	internal class RendererHelper
	{
		private readonly Label _label;
		private readonly string _runtimePlatform;
		private readonly bool _isRtl;
		private readonly string _text;
		private readonly IList<KeyValuePair<string, string>> _styles;
		private static readonly string[] SupportedProperties = {
				Label.TextProperty.PropertyName,
				Label.FontAttributesProperty.PropertyName,
				Label.FontFamilyProperty.PropertyName,
				Label.FontSizeProperty.PropertyName,
				Label.HorizontalTextAlignmentProperty.PropertyName,
				Label.TextColorProperty.PropertyName,
                Label.PaddingProperty.PropertyName,
				HtmlLabel.LinkColorProperty.PropertyName
			};

		public RendererHelper(Label label, string text, string runtimePlatform, bool isRtl)
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
			string GetSystemFont() => _runtimePlatform switch
			{
				Device.iOS => "-apple-system",
				Device.Android => "Roboto",
				Device.UWP => "Segoe UI",
				_ => "system-ui",
			};

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
			if (color.IsDefault)
			{
				return;
			}

			var red = (int)(color.R * 255);
			var green = (int)(color.G * 255);
			var blue = (int)(color.B * 255);
			var alpha = color.A;
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
		public static bool HandleUriClick(HtmlLabel label, string url)
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
                if (label.IsExternalBrowser)
                {
                    uri.LaunchBrowser(label.BrowserLaunchOptions);
                }
                else
                {
                    label.RaiseLinkClickEvent(url);
                }
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
