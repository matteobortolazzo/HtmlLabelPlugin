using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("HtmlLabel.Forms.Plugin.Shared.Tests")]
namespace LabelHtml.Forms.Plugin.Abstractions
{
    internal class RendererHelper
    {
		private readonly Label _label;
		private readonly string _text;
		private readonly IList<KeyValuePair<string, string>> _styles;
		
		public RendererHelper(Label label, string text)
		{
			_label = label ?? throw new ArgumentNullException(nameof(label));
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

		public void AddFontFamilyStyle(string fontFamily, bool includeAppleSystem = false)
        {
			var fontFamilyValue = includeAppleSystem ?
				"-apple-system," :
				string.Empty;
			fontFamilyValue += fontFamily;

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
			var alpha = (int)(color.A * 255);
			var hex = $"#{red:X2}{green:X2}{blue:X2}{alpha:X2}";
			AddStyle("color", hex);
		}

		public void AddHorizontalTextAlignStyle(TextAlignment textAlignment)
		{
			if (textAlignment == TextAlignment.Center)
			{
				AddStyle("text-align", "center");
			}
			else if (textAlignment == TextAlignment.End)
			{
				AddStyle("text-align", "right");
				AddStyle("text-align", "end");
			}
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool isAppleSystem)
		{
			if (string.IsNullOrWhiteSpace(_text))
            {
                return string.Empty;
            }
			            
			AddFontAttributesStyle(_label.FontAttributes);
			AddFontFamilyStyle(_label.FontFamily, isAppleSystem);
			AddTextColorStyle(_label.TextColor);
			AddHorizontalTextAlignStyle(_label.HorizontalTextAlignment);

			if (_label.FontSize != Device.GetNamedSize(NamedSize.Default, typeof(Label)))
			{
				AddFontSizeStyle(_label.FontSize);
			}

			var style = GetStyle();
			return $"<div style=\"{style}\">{_text}</div>";
		}

		public string GetStyle()
		{
			var builder = new StringBuilder();

			foreach (KeyValuePair<string, string> style in _styles)
			{
				_ = builder.Append($"{style.Key}:{style.Value},");
			}

			return builder.ToString();
		}

		private void AddStyle(string selector, string value)
		{
			_styles.Add(new KeyValuePair<string, string>(selector, value));
		}
	}
}