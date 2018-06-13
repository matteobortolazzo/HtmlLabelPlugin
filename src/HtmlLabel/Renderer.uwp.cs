using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using LabelHtml.Forms.Plugin.Abstractions;
using Xamarin.Forms.Platform.UWP;
using LabelHtml.Forms.Plugin.UWP;
using Microsoft.Xaml.Interactivity;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Span = Windows.UI.Xaml.Documents.Span;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
// ReSharper disable once CheckNamespace
namespace LabelHtml.Forms.Plugin.UWP
{
	/// <summary>
	/// HtmlLable Implementation
	/// </summary>
	[Preserve(AllMembers = true)]
	public class HtmlLabelRenderer : LabelRenderer
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Initialize() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control == null) return;

			UpdateText();
			UpdateMaxLines();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == HtmlLabel.MaxLinesProperty.PropertyName)
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
			var maxLines = HtmlLabel.GetMaxLines(Element);
			if (maxLines == default(int)) return;
			Control.MaxLines = maxLines;
		}

		private void UpdateText()
		{
			if (Control == null || Element == null) return;

			if (string.IsNullOrEmpty(Control.Text)) return;

			// Gets the complete HTML string
			var helper = new LabelRendererHelper(Element, Element.Text);
			Control.Text = helper.ToString();

			// Adds the HtmlTextBehavior because UWP's TextBlock
			// does not natively support HTML content
			var behavior = new HtmlTextBehavior((HtmlLabel)Element);
			var behaviors = Interaction.GetBehaviors(Control);
			behaviors.Clear();
			behaviors.Add(behavior);
		}
	}

	internal class HtmlTextBehavior : Behavior<TextBlock>
	{
		// All the supported tags
		internal const string ElementA = "A";
		internal const string ElementB = "B";
		internal const string ElementBr = "BR";
		internal const string ElementEm = "EM";
		internal const string ElementI = "I";
		internal const string ElementP = "P";
		internal const string ElementStrong = "STRONG";
		internal const string ElementU = "U";
		internal const string ElementUl = "UL";
		internal const string ElementLi = "LI";
		internal const string ElementDiv = "DIV";

		private readonly HtmlLabel _label;
		public HtmlTextBehavior(HtmlLabel label)
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

		private void OnAssociatedObjectLayoutUpdated(object sender, object o) => UpdateText();

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

		private static void ParseText(XElement element, InlineCollection inlines, HtmlLabel label)
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
					link.Click += (sender, e) =>
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

		public void Detach() => OnDetaching();

		protected virtual void OnAttached() { }

		protected virtual void OnDetaching() { }

		protected DependencyObject AssociatedObject { get; set; }

		DependencyObject IBehavior.AssociatedObject => AssociatedObject;
	}
}