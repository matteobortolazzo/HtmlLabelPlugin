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
			_text = text?.Trim() ?? throw new ArgumentNullException(nameof(text));
			_styles = new List<KeyValuePair<string, string>>();
		}

		public void SetFontAttributes()
		{
			if (_label.FontAttributes == FontAttributes.Bold)
            {
				AddStyle("font-weight", "bold");
			}
			else if (_label.FontAttributes == FontAttributes.Italic)
			{
				AddStyle("font-style", "italic");
			}
		}

		public void SetFontFamily(bool includeAppleSystem = false)
        {
			var fontFamily = includeAppleSystem ?
				"-apple-system, " :
				string.Empty;
			fontFamily += _label.FontFamily;

			AddStyle("font-family", $"'{fontFamily}'");
        }

		public void SetFontSize()
		{
			AddStyle("font-size", $"{_label.FontSize}px");
		}

		public void SetTextColor()
		{
			if (_label.TextColor.IsDefault)
            {
                return;
            }

            Color color = _label.TextColor;
			var red = (int)(color.R * 255);
			var green = (int)(color.G * 255);
			var blue = (int)(color.B * 255);
			var alpha = (int)(color.A * 255);
			var hex = $"#{red:X2}{green:X2}{blue:X2}{alpha:X2}";
			AddStyle("color", hex);
		}

		public void SetHorizontalTextAlign()
		{
			if (_label.HorizontalTextAlignment == TextAlignment.Center)
			{
				AddStyle("text-align", "center");
			}
			else if (_label.HorizontalTextAlignment == TextAlignment.End)
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
			if (string.IsNullOrWhiteSpace(_label.Text))
            {
                return string.Empty;
            }
			            
			SetFontAttributes();
			SetFontFamily(isAppleSystem);
			SetFontSize();
			SetTextColor();
			SetHorizontalTextAlign();

			var builder = new StringBuilder();

			_ = builder.Append("<div style=\"");

			foreach (KeyValuePair<string, string> style in _styles)
			{
				_ = builder.Append($"{style.Key}:{style.Value},");
			}

			_ = builder.Append($"\">{_text}</div>");
			var text = builder.ToString();
			return text;
		}

		private void AddStyle(string selector, string value)
		{
			_styles.Add(new KeyValuePair<string, string>(selector, value));
		}
	}
}