using System.Text.RegularExpressions;
using System.Xml.Linq;
using LabelHtml.Forms.Plugin.Abstractions;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using Microsoft.Maui.Platform;
using PlatformView = Microsoft.UI.Xaml.Controls.TextBlock;
using Span = Microsoft.UI.Xaml.Documents.Span;

namespace LabelHtml.Forms.Plugin.UWP
{
	internal class HtmlTextBehavior : Behavior<PlatformView>
	{
		// All the supported tags
		internal const string _elementA = "A";
		internal const string _elementB = "B";
		internal const string _elementBr = "BR";
		internal const string _elementEm = "EM";
		internal const string _elementI = "I";
		internal const string _elementP = "P";
		internal const string _elementStrong = "STRONG";
		internal const string _elementU = "U";
		internal const string _elementUl = "UL";
		internal const string _elementLi = "LI";
		internal const string _elementDiv = "DIV";

		public IHtmlLabel HtmlLabel { get; set; }

		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Loaded += OnAssociatedObjectLoaded;
			AssociatedObject.LayoutUpdated += OnAssociatedObjectLayoutUpdated;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			AssociatedObject.LayoutUpdated -= OnAssociatedObjectLayoutUpdated;
		}

		private void OnAssociatedObjectLayoutUpdated(object sender, object o) => UpdateText();

		private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) => UpdateText();

		private void UpdateText()
		{
			if (AssociatedObject == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(AssociatedObject.Text))
			{
				return;
			}

			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			AssociatedObject.LayoutUpdated -= OnAssociatedObjectLayoutUpdated;

			var text = AssociatedObject.Text;

			// Just incase we are not given text with elements.
			var modifiedText = $"<div>{text}</div>";

			var linkRegex = new Regex(@"<a\s+href=""(.+?)\""");

			MatchCollection matches = linkRegex.Matches(modifiedText);
			if (matches.Count > 0)
			{
				foreach (Match match in matches)
				{
					for (var i = 1; i < match.Groups.Count; i++)
					{
						Group group = match.Groups[i];
						var escapedUri = Uri.EscapeDataString(group.Value);
						modifiedText = modifiedText.Replace(group.Value, escapedUri, StringComparison.InvariantCulture);
					}
				}
				System.Diagnostics.Debug.WriteLine(@$"ERROR: ${matches}");
			}

			modifiedText = Regex.Replace(modifiedText, "<br>", "<br></br>", RegexOptions.IgnoreCase)
				.Replace("\n", String.Empty, StringComparison.OrdinalIgnoreCase)  // KWI-FIX Enters resulted in multiple lines
				.Replace("&nbsp;", "&#160;", StringComparison.OrdinalIgnoreCase); // KWI-FIX &nbsp; is not supported by the UWP TextBlock
			
			// reset the text because we will add to it.
			AssociatedObject.Inlines.Clear();

			var element = XElement.Parse(modifiedText);
			ParseText(element, AssociatedObject.Inlines, HtmlLabel);
		}

		private static void ParseText(XElement element, InlineCollection inlines, IHtmlLabel label)
		{
			if (element == null)
			{
				return;
			}

			InlineCollection currentInlines = inlines;
			var elementName = element.Name.ToString().ToUpperInvariant();
			switch (elementName)
			{
				case _elementA:
					var link = new Hyperlink();
					XAttribute href = element.Attribute("href");
					var unescapedUri = Uri.UnescapeDataString(href?.Value);
					if (href != null)
					{
						try
						{
							link.NavigateUri = new Uri(unescapedUri);
						}
						catch (FormatException) { /* href is not valid */ }
					}
					link.Click += (sender, e) =>
					{
						sender.NavigateUri = null;
						RendererHelper.HandleUriClick(label, unescapedUri);
					};
					if ( !ControlsColorExtensions.IsDefault( label.LinkColor ) )
					{
						link.Foreground = label.LinkColor.ToPlatform();
					}
					if (!label.UnderlineText)
					{
						link.UnderlineStyle = UnderlineStyle.None;
					}
					inlines.Add(link);
					currentInlines = link.Inlines;
					break;
				case _elementB:
				case _elementStrong:
					var bold = new Bold();
					inlines.Add(bold);
					currentInlines = bold.Inlines;
					break;
				case _elementI:
				case _elementEm:
					var italic = new Italic();
					inlines.Add(italic);
					currentInlines = italic.Inlines;
					break;
				case _elementU:
					var underline = new Underline();
					inlines.Add(underline);
					currentInlines = underline.Inlines;
					break;
				case _elementBr:
					inlines.Add(new LineBreak());
					break;
				case _elementP:
					// Add two line breaks, one for the current text and the second for the gap.
					if (AddLineBreakIfNeeded(inlines))
					{
						inlines.Add(new LineBreak());
					}

					var paragraphSpan = new Span();
					inlines.Add(paragraphSpan);
					currentInlines = paragraphSpan.Inlines;
					break;
				case _elementLi:
					inlines.Add(new LineBreak());
					inlines.Add(new Run { Text = " • " });
					break;
				case _elementUl:
				case _elementDiv:
					_ = AddLineBreakIfNeeded(inlines);
					var divSpan = new Span();
					inlines.Add(divSpan);
					currentInlines = divSpan.Inlines;
					break;
			}
			foreach (XNode node in element.Nodes())
			{
				if (node is XText textElement)
				{
					currentInlines.Add(new Run { Text = textElement.Value });
				}
				else
				{
					ParseText(node as XElement, currentInlines, label);
				}
			}

			// Add newlines for paragraph tags
			if (elementName == "ElementP")
			{
				currentInlines.Add(new LineBreak());
			}
		}
		private static bool AddLineBreakIfNeeded(InlineCollection inlines)
		{
			if (inlines.Count <= 0)
			{
				return false;
			}

			Inline lastInline = inlines[inlines.Count - 1];
			while ((lastInline is Span))
			{
				var span = (Span)lastInline;
				if (span.Inlines.Count > 0)
				{
					lastInline = span.Inlines[span.Inlines.Count - 1];
				}
			}

			if (lastInline is LineBreak)
			{
				return false;
			}

			inlines.Add(new LineBreak());
			return true;
		}
	}
}