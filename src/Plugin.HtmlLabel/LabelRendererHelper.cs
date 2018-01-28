using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Plugin.HtmlLabel
{
    public class LabelRendererHelper
    {
        private readonly Label _label;
        private readonly string _text;
        private readonly StringBuilder _builder;

        public LabelRendererHelper(Label label, string text)
        {
            _label = label;
            _text = text;
            _builder = new StringBuilder();
        }

        private void SetFontAttributes()
        {
            if (_label.FontAttributes == FontAttributes.None) return;
            switch (_label.FontAttributes)
            {
                case FontAttributes.Bold:
                    _builder.Append("font-weight: bold; ");
                    break;
                case FontAttributes.Italic:
                    _builder.Append("font-style: italic; ");
                    break;
            }
        }
        private void SetFontFamily()
        {
            if (_label.FontFamily == null) return;
            _builder.Append($"font-family: '{_label.FontFamily}'; ");
        }

        private void SetFontSize()
        {
            if (Math.Abs(_label.FontSize - 14d) < 0.000000001) return;
            _builder.Append($"font-size: {_label.FontSize}px; ");
        }

        private void SetTextColor()
        {
            if (_label.TextColor.IsDefault) return;
            var color = _label.TextColor;
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{red:X2}{green:X2}{blue:X2}{alpha:X2}";
            _builder.Append($"color: {hex}; ");
        }

        private void SetHorizontalTextAlign()
        {
            if (_label.HorizontalTextAlignment == TextAlignment.Start) return;
            switch (_label.HorizontalTextAlignment)
            {
                case TextAlignment.Center:
                    _builder.Append("text-align: center; ");
                    break;
                case TextAlignment.End:
                    _builder.Append("text-align: right; ");
                    break;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_label.Text))
                return string.Empty;

            _builder.Append("<div style=\"");
            SetFontAttributes();
            SetFontFamily();
            SetFontSize();
            SetTextColor();
            SetHorizontalTextAlign();
            _builder.Append($"\">{_text}</div>");
            var text = _builder.ToString();
            return text;
        }
    }
}
