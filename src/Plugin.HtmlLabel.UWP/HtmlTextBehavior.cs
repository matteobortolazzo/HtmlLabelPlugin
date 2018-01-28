using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Plugin.HtmlLabel.UWP
{
    public class HtmlTextBehavior : Behavior<TextBlock>
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

            string text = AssociatedObject.Text;

            // Just incase we are not given text with elements.
            string modifiedText = string.Format("<div>{0}</div>", text);
            modifiedText = Regex.Replace(modifiedText, "<br>", "<br></br>", RegexOptions.IgnoreCase);
            // reset the text because we will add to it.
            AssociatedObject.Inlines.Clear();
            try
            {
                var element = XElement.Parse(modifiedText);
                ParseText(element, AssociatedObject.Inlines);
            }
            catch (Exception)
            {
                // if anything goes wrong just show the html
                AssociatedObject.Text = text;
            }
            AssociatedObject.LayoutUpdated -= OnAssociatedObjectLayoutUpdated;
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
        }
        private static void ParseText(XElement element, InlineCollection inlines)
        {
            if (element == null) return;

            InlineCollection currentInlines = inlines;
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
                        catch (System.FormatException) { /* href is not valid */ }
                    }
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

                    Span paragraphSpan = new Span();
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
                    Span divSpan = new Span();
                    inlines.Add(divSpan);
                    currentInlines = divSpan.Inlines;
                    break;
            }
            foreach (var node in element.Nodes())
            {
                XText textElement = node as XText;
                if (textElement != null)
                {
                    currentInlines.Add(new Run { Text = textElement.Value });
                }
                else
                {
                    ParseText(node as XElement, currentInlines);
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
            if (inlines.Count > 0)
            {
                var lastInline = inlines[inlines.Count - 1];
                while ((lastInline is Span))
                {
                    var span = (Span)lastInline;
                    if (span.Inlines.Count > 0)
                    {
                        lastInline = span.Inlines[span.Inlines.Count - 1];
                    }
                }
                if (!(lastInline is LineBreak))
                {
                    inlines.Add(new LineBreak());
                    return true;
                }
            }
            return false;
        }
    }
}
