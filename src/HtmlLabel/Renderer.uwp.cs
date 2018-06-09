using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using HtmlLabel.Forms.Plugin.Abstractions;
using Xamarin.Forms.Platform.UWP;
using HtmlLabel.Forms.Plugin.UWP;
using Microsoft.Xaml.Interactivity;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Span = Windows.UI.Xaml.Documents.Span;

[assembly: ExportRenderer(typeof(LabelHtml), typeof(HtmlLabelRenderer))]
// ReSharper disable once CheckNamespace
namespace HtmlLabel.Forms.Plugin.UWP
{
	/// <summary>
	/// HtmlLable Implementation
	/// </summary>
	[Preserve(AllMembers = true)]
	public class HtmlLabelRenderer : LabelRenderer
	{
		public static void Initialize() { }

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control == null) return;

			UpdateText();
			UpdateMaxLines();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == LabelHtml.MaxLinesProperty.PropertyName)
				UpdateMaxLines();
			else if (e.PropertyName == Label.TextProperty.PropertyName ||
			         e.PropertyName == Label.FontAttributesProperty.PropertyName ||
			         e.PropertyName == Label.FontFamilyProperty.PropertyName ||
			         e.PropertyName == Label.FontSizeProperty.PropertyName ||
			         e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
			         e.PropertyName == Label.TextColorProperty.PropertyName)
				UpdateText();
		}

		private void UpdateMaxLines()
		{
			var maxLines = LabelHtml.GetMaxLines(Element);
			if (maxLines == default(int)) return;
			Control.MaxLines = maxLines;
		}

		private void UpdateText()
		{
			if (Control == null || Element == null) return;

			if (string.IsNullOrEmpty(Control.Text)) return;

			var helper = new LabelRendererHelper(Element, Element.Text);
			Control.Text = helper.ToString();

			var behavior = new HtmlTextBehavior((LabelHtml)Element);
			var behaviors = Interaction.GetBehaviors(Control);
			behaviors.Clear();
			behaviors.Add(behavior);
		}
	}

	internal class HtmlTextBehavior : Behavior<TextBlock>
	{
		private const string ElementA = "A";
		private const string ElementB = "B";
		private const string ElementBr = "BR";
		private const string ElementEm = "EM";
		private const string ElementI = "I";
		private const string ElementP = "P";
		private const string ElementStrong = "STRONG";
		private const string ElementU = "U";
		private const string ElementUl = "UL";
		private const string ElementLi = "LI";
		private const string ElementDiv = "DIV";

		private readonly LabelHtml _label;
		public HtmlTextBehavior(LabelHtml label)
		{
			_label = label;
		}

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

		private void OnAssociatedObjectLayoutUpdated(object sender, object o)
		{
			UpdateText();
		}

		private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			UpdateText();
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
		}

		private void UpdateText()
		{
			if (AssociatedObject == null) return;
			if (string.IsNullOrEmpty(AssociatedObject.Text)) return;

			var text = AssociatedObject.Text;

			// Just incase we are not given text with elements.
			var modifiedText = string.Format("<div>{0}</div>", text);
			modifiedText = Regex.Replace(modifiedText, "<br>", "<br></br>", RegexOptions.IgnoreCase);
			// reset the text because we will add to it.
			AssociatedObject.Inlines.Clear();
			try
			{
				var element = XElement.Parse(modifiedText);
				ParseText(element, AssociatedObject.Inlines, _label);
			}
			catch (Exception)
			{
				// if anything goes wrong just show the html
				AssociatedObject.Text = text;
			}
			AssociatedObject.LayoutUpdated -= OnAssociatedObjectLayoutUpdated;
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
		}

		private static void ParseText(XElement element, InlineCollection inlines, LabelHtml label)
		{
			if (element == null) return;

			var currentInlines = inlines;
			var elementName = element.Name.ToString().ToUpper();
			switch (elementName)
			{
				case ElementA:
					var link = new Hyperlink();
					var href = element.Attribute("href");
					if (href != null)
					{
						try
						{
							link.NavigateUri = new Uri(href.Value);
						}
						catch (FormatException) { /* href is not valid */ }
					}
					link.Click += (Hyperlink sender, HyperlinkClickEventArgs e) =>
					{
						sender.NavigateUri = null;
						if (href == null) return;

						var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = href.Value }, href.Value);
						label.SendNavigating(args);

						if (args.Cancel)
							return;

						Device.OpenUri(new Uri(href.Value));
						label.SendNavigated(args);
					};
					inlines.Add(link);
					currentInlines = link.Inlines;
					break;
				case ElementB:
				case ElementStrong:
					var bold = new Bold();
					inlines.Add(bold);
					currentInlines = bold.Inlines;
					break;
				case ElementI:
				case ElementEm:
					var italic = new Italic();
					inlines.Add(italic);
					currentInlines = italic.Inlines;
					break;
				case ElementU:
					var underline = new Underline();
					inlines.Add(underline);
					currentInlines = underline.Inlines;
					break;
				case ElementBr:
					inlines.Add(new LineBreak());
					break;
				case ElementP:
					// Add two line breaks, one for the current text and the second for the gap.
					if (AddLineBreakIfNeeded(inlines))
					{
						inlines.Add(new LineBreak());
					}

					var paragraphSpan = new Span();
					inlines.Add(paragraphSpan);
					currentInlines = paragraphSpan.Inlines;
					break;
				case ElementLi:
					inlines.Add(new LineBreak());
					inlines.Add(new Run { Text = " • " });
					break;
				case ElementUl:
				case ElementDiv:
					AddLineBreakIfNeeded(inlines);
					var divSpan = new Span();
					inlines.Add(divSpan);
					currentInlines = divSpan.Inlines;
					break;
			}
			foreach (var node in element.Nodes())
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
			if (inlines.Count <= 0) return false;

			var lastInline = inlines[inlines.Count - 1];
			while ((lastInline is Span))
			{
				var span = (Span)lastInline;
				if (span.Inlines.Count > 0)
				{
					lastInline = span.Inlines[span.Inlines.Count - 1];
				}
			}

			if (lastInline is LineBreak) return false;

			inlines.Add(new LineBreak());
			return true;
		}
	}

	internal abstract class Behavior<T> : Behavior where T : DependencyObject
	{
		protected new T AssociatedObject => base.AssociatedObject as T;

		protected override void OnAttached()
		{
			base.OnAttached();
			if (AssociatedObject == null) throw new InvalidOperationException("AssociatedObject is not of the right type");
		}
	}

	internal abstract class Behavior : DependencyObject, IBehavior
	{
		public void Attach(DependencyObject associatedObject)
		{
			AssociatedObject = associatedObject;
			OnAttached();
		}

		public void Detach()
		{
			OnDetaching();
		}

		protected virtual void OnAttached() { }

		protected virtual void OnDetaching() { }

		protected DependencyObject AssociatedObject { get; set; }

		DependencyObject IBehavior.AssociatedObject => this.AssociatedObject;
	}
}