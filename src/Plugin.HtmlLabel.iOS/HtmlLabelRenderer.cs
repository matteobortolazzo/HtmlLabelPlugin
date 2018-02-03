using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Plugin.HtmlLabel;
using Plugin.HtmlLabel.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace Plugin.HtmlLabel.iOS
{
    public class HtmlLabelRenderer : LabelRenderer
    {
        private class LinkData
        {
            public LinkData(NSRange range, string url) { Range = range; Url = url; }
            public NSRange Range;
            public readonly string Url;
        }

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
            if (e.PropertyName == HtmlLabel.MaxLinesProperty.PropertyName)
                UpdateMaxLines();
            else if (e.PropertyName == Label.TextProperty.PropertyName ||
                e.PropertyName == HtmlLabel.IsHtmlProperty.PropertyName ||
                e.PropertyName == HtmlLabel.RemoveHtmlTagsProperty.PropertyName ||
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
            Control.Lines = maxLines;

            SetNeedsDisplay();
        }

        private void UpdateText()
        {
            if (Control == null || Element == null) return;

            var isHtml = HtmlLabel.GetIsHtml(Element);
            if (!isHtml) return;

            var removeTags = HtmlLabel.GetRemoveHtmlTags(Element);

            var text = removeTags ? 
                HtmlToText.ConvertHtml(Control.Text) : 
                Element.Text;

            var helper = new LabelRendererHelper(Element, text);

            CreateAttributedString(Control, helper.ToString());
            SetNeedsDisplay();
        }

        private void CreateAttributedString(UILabel control, string html)
        {
            var attr = new NSAttributedStringDocumentAttributes();
            var nsError = new NSError();
            attr.DocumentType = NSDocumentType.HTML;
            // --------------
            // 02-01-2018 : Fix for default font family => https://github.com/matteobortolazzo/HtmlLabelPlugin/issues/9
            var fontDescriptor = control.Font.FontDescriptor.VisibleName;
            var fontFamily = fontDescriptor.ToLower().Contains("system") ? "-apple-system,system-ui,BlinkMacSystemFont,Segoe UI" : control.Font.FamilyName;
            html += "<style> body{ font-family: " + fontFamily + ";}</style>"; 
            // --------------
            var myHtmlData = NSData.FromString(html, NSStringEncoding.Unicode);
            // control.Lines = 0;
            var mutable = new NSMutableAttributedString(new NSAttributedString(myHtmlData, attr, ref nsError));
            var links = new List<LinkData>();
            control.AttributedText = mutable;

            // make a list of all links:
            mutable.EnumerateAttributes(new NSRange(0, mutable.Length), NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, (NSDictionary attrs, NSRange range, ref bool stop) =>
            {
                foreach (var a in attrs) // should use attrs.ContainsKey(something) instead
                {
                    if (a.Key.ToString() != "NSLink") continue;
                    links.Add(new LinkData(range, a.Value.ToString()));
                    return;
                }
            });

            // Set up a Gesture recognizer:
            if (links.Count <= 0) return;
            control.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer((tap) =>
            {
                var url = DetectTappedUrl(tap, (UILabel)tap.View, links);
                if (url != null)
                {
                    // open the link:
                    Device.OpenUri(new Uri(url));
                }
            });
            control.AddGestureRecognizer(tapGesture);
        }

        private string DetectTappedUrl(UIGestureRecognizer tap, UILabel label, IEnumerable<LinkData> linkList)
        {
            // Create instances of NSLayoutManager, NSTextContainer and NSTextStorage
            var layoutManager = new NSLayoutManager();
            var textContainer = new NSTextContainer();
            var textStorage = new NSTextStorage();
            textStorage.SetString(label.AttributedText);

            // Configure layoutManager and textStorage
            layoutManager.AddTextContainer(textContainer);
            textStorage.AddLayoutManager(layoutManager);

            // Configure textContainer
            textContainer.LineFragmentPadding = 0;
            textContainer.LineBreakMode = label.LineBreakMode;
            textContainer.MaximumNumberOfLines = (nuint)label.Lines;
            var labelSize = label.Bounds.Size;
            textContainer.Size = labelSize;

            // Find the tapped character location and compare it to the specified range
            var locationOfTouchInLabel = tap.LocationInView(label);
            var textBoundingBox = layoutManager.GetUsedRectForTextContainer(textContainer);
            var textContainerOffset = new CGPoint((labelSize.Width - textBoundingBox.Size.Width) * 0.0 - textBoundingBox.Location.X,
                (labelSize.Height - textBoundingBox.Size.Height) * 0.0 - textBoundingBox.Location.Y);

            nfloat labelX;
            switch (Element.HorizontalTextAlignment)
            {
                case TextAlignment.End:
                    labelX = locationOfTouchInLabel.X - (labelSize.Width - textBoundingBox.Size.Width);
                    break;
                case TextAlignment.Center:
                    labelX = locationOfTouchInLabel.X - (labelSize.Width - textBoundingBox.Size.Width) / 2;
                    break;
                default:
                    labelX = locationOfTouchInLabel.X;
                    break;
            }
            var locationOfTouchInTextContainer = new CGPoint(labelX - textContainerOffset.X, locationOfTouchInLabel.Y - textContainerOffset.Y);

            nfloat partialFraction = 0;
            var indexOfCharacter = (nint)layoutManager.CharacterIndexForPoint(locationOfTouchInTextContainer, textContainer, ref partialFraction);

            foreach (var link in linkList)
            {
                // Xamarin version of NSLocationInRange?
                if ((indexOfCharacter >= link.Range.Location) && (indexOfCharacter < link.Range.Location + link.Range.Length))
                {
                    return link.Url;
                }
            }
            return null;
        }
    }
}