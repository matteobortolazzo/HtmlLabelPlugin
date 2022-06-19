﻿using Foundation;
using HyperTextLabel.Maui.Controls;
using HyperTextLabel.Maui.Utilities;
using Microsoft.Maui.Platform;
using UIKit;

namespace HyperTextLabel.Maui.Platforms.iOS
{
    internal static class HtmlLabelExtensions
    {
        public static void UpdateText(this MauiLabel view, IHtmlLabel label, IFontManager fontManager)
        {
            if (string.IsNullOrWhiteSpace(label?.Text))
            {
                view.Text = string.Empty;
                return;
            }

            var uiFont = fontManager.GetFont(label.Font, UIFont.LabelFontSize);
            view.Font = uiFont;

            var linkColor = label.LinkColor;
            if (!linkColor.IsDefault())
            {
                view.TintColor = linkColor.ToPlatform();
            }
            var isRtl = AppInfo.RequestedLayoutDirection == LayoutDirection.RightToLeft;
            var styledHtml = new RendererHelper(label, label.Text, DevicePlatform.iOS, isRtl).ToString();
            SetText(styledHtml, view, label);
            view.SetNeedsDisplay();
        }

        public static void UpdateUnderlineText(this MauiLabel view, IHtmlLabel label)
        {
        }

        public static void UpdateLinkColor(this MauiLabel view, IHtmlLabel label)
        {
        }

        public static void UpdateBrowserLaunchOptions(this MauiLabel view, IHtmlLabel label)
        {
        }

        public static void UpdateAndroidLegacyMode(this MauiLabel view, IHtmlLabel label)
        {
        }

        public static void UpdateAndroidListIndent(this MauiLabel view, IHtmlLabel label)
        {
        }

        private static void SetText(string html, MauiLabel view, IHtmlLabel label)
        {
            // Create HTML data sting
            var stringType = new NSAttributedStringDocumentAttributes
            {
                DocumentType = NSDocumentType.HTML,
                StringEncoding = NSStringEncoding.UTF8
            };
            var nsError = new NSError();

            var htmlData = NSData.FromString(html, NSStringEncoding.Unicode);

            using var htmlString = new NSAttributedString(htmlData, stringType, out _, ref nsError);
            var mutableHtmlString = htmlString.RemoveTrailingNewLines();

            mutableHtmlString.EnumerateAttributes(new NSRange(0, mutableHtmlString.Length), NSAttributedStringEnumeration.None,
                (NSDictionary value, NSRange range, ref bool stop) =>
                {
                    try
                    {
                        var md = new NSMutableDictionary(value);
                        var font = md[UIStringAttributeKey.Font] as UIFont;

                        if (font != null)
                        {
                            md[UIStringAttributeKey.Font] = view.Font.WithTraitsOfFont(font);
                        }
                        else
                        {
                            md[UIStringAttributeKey.Font] = view.Font;
                        }

                        var foregroundColor = md[UIStringAttributeKey.ForegroundColor] as UIColor;
                        if (foregroundColor == null || foregroundColor.IsEqualToColor(UIColor.Black))
                        {
                            md[UIStringAttributeKey.ForegroundColor] = view.TextColor;
                        }

                        mutableHtmlString.SetAttributes(md, range);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        throw;
                    }
                });

            mutableHtmlString.SetLineHeight(label);
            mutableHtmlString.SetLinksStyles(label);
            view.AttributedText = mutableHtmlString;
        }

        //private static bool NavigateToUrl(NSUrl url)
        //{
        //    if (url == null)
        //    {
        //        throw new ArgumentNullException(nameof(url));
        //    }
        //    // Try to handle uri, if it can't be handled, fall back to IOS his own handler.
        //    return !RendererHelper.HandleUriClick(Element, url.AbsoluteString);
        //}

    }
}
