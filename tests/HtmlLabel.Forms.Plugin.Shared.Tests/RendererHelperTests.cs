using LabelHtml.Forms.Plugin.Abstractions;
using System;
using System.Reflection;
using Xamarin.Forms;
using Xunit;

namespace LabelHtml.Forms.Plugin.Shared.Tests
{
    public class RendererHelperTests
    {
        private readonly RendererHelper _helper;

        public RendererHelperTests()
        {
            var label = new Label();
            var text = Guid.NewGuid().ToString();
            _helper = new RendererHelper(label, text, "iOS", false);
        }

        [Fact]
        public void AddFontAttributesStyle_WithNone_ShouldNotSet()
        {
            // Act
            _helper.AddFontAttributesStyle(FontAttributes.None);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void AddFontAttributesStyle_WithBold_ShouldSet_FontWeightBold()
        {
            var expected = "font-weight:bold";

            // Act
            _helper.AddFontAttributesStyle(FontAttributes.Bold);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddFontAttributesStyle_WithItalic_ShouldSet_FontStyleItalic()
        {
            var expected = "font-style:italic";

            // Act
            _helper.AddFontAttributesStyle(FontAttributes.Italic);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddFontFamilyStyle_ShouldSet_FontFamily()
        {
            var fontFamily = Guid.NewGuid().ToString();
            var expected = $"font-family:'-apple-system,{fontFamily}'";

            // Act
            _helper.AddFontFamilyStyle(fontFamily);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddFontSizeStyle_ShouldSet_FontSize()
        {
            var fontSize = new Random().Next();
            var expected = $"font-size:{fontSize}px";

            // Act
            _helper.AddFontSizeStyle(fontSize);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddTextColorStyle_WithDefault_ShouldNotSet()
        {
            var color = new Color();

            // Act
            _helper.AddTextColorStyle(color);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void AddTextColorStyle_ShouldSet_Color()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var red = rnd.Next(255);
            var green = rnd.Next(255);
            var blue = rnd.Next(255);
            var alpha = rnd.Next(255) / 100;
            var color = new Color(red / 255d, green / 255d, blue / 255d, alpha / 255d);
            var expected = $"color:#{red:X2}{green:X2}{blue:X2};color:rgba({red},{green},{blue},{alpha})";

            // Act
            _helper.AddTextColorStyle(color);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddHorizontalTextAlignStyle_WithStart_ShouldNotSet()
        {
            var expected = "text-align:left";

            // Act
            _helper.AddHorizontalTextAlignStyle(TextAlignment.Start);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddHorizontalTextAlignStyle_WithCenter_ShouldSet_TextAlignCenter()
        {
            var expected = "text-align:center";

            // Act
            _helper.AddHorizontalTextAlignStyle(TextAlignment.Center);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddHorizontalTextAlignStyle_WithEnd_ShouldSet_TextAlignRightEnd()
        {
            var expected = "text-align:right";

            // Act
            _helper.AddHorizontalTextAlignStyle(TextAlignment.End);
            var actual = _helper.GetStyle();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
