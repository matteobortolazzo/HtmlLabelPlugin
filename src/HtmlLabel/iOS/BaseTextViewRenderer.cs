using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace LabelHtml.Forms.Plugin.iOS
{
    public abstract class BaseTextViewRenderer<TElement> : ViewRenderer<TElement, UITextViewFixedWithKludge>
		where TElement : Label
	{
        private SizeRequest _perfectSize;
        private bool _perfectSizeValid;

		protected override UITextViewFixedWithKludge CreateNativeControl()
		{
            var control = new UITextViewFixedWithKludge(CGRect.Empty)
            {
                Editable = false,
                ScrollEnabled = false,
                ShowsVerticalScrollIndicator = false,
                BackgroundColor = UIColor.Clear
            };
            return control;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			if (e == null || Element == null)
			{
				return;
			}

			if (e.NewElement != null)
			{
				try
				{
					if (Control == null)
					{
						SetNativeControl(CreateNativeControl());
					}
					bool shouldInteractWithUrl = !Element.GestureRecognizers.Any();
					if (shouldInteractWithUrl)
					{
						// Setting the data detector types mask to capture all types of link-able data
						Control.DataDetectorTypes = UIDataDetectorType.All;
						Control.Selectable = true;
						Control.Delegate = new TextViewDelegate(NavigateToUrl);
					}
					else
					{
						Control.Selectable = false;
						foreach (var recognizer in this.Element.GestureRecognizers.OfType<TapGestureRecognizer>())
						{
							if (recognizer.Command != null)
							{
								var command = recognizer.Command;
								Control.AddGestureRecognizer(new UITapGestureRecognizer(() => { command.Execute(recognizer.CommandParameter); }));
							}
						}
					}
					UpdateLineBreakMode();
					UpdateHorizontalTextAlignment();
					ProcessText();
					UpdatePadding();
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
				}
			}
			base.OnElementChanged(e);
		}
		protected abstract void ProcessText();
		protected abstract bool NavigateToUrl(NSUrl url);

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (!_perfectSizeValid)
			{
				_perfectSize = base.GetDesiredSize(double.PositiveInfinity, double.PositiveInfinity);
				_perfectSize.Minimum = new Size(Math.Min(10, _perfectSize.Request.Width), _perfectSize.Request.Height);
				_perfectSizeValid = true;
			}

			var widthFits = widthConstraint >= _perfectSize.Request.Width;
			var heightFits = heightConstraint >= _perfectSize.Request.Height;

			if (widthFits && heightFits)
				return _perfectSize;

			var result = base.GetDesiredSize(widthConstraint, heightConstraint);
			var tinyWidth = Math.Min(10, result.Request.Width);
			result.Minimum = new Size(tinyWidth, result.Request.Height);

			if (widthFits || Element.LineBreakMode == LineBreakMode.NoWrap)
				return result;

			var containerIsNotInfinitelyWide = !double.IsInfinity(widthConstraint);

			if (containerIsNotInfinitelyWide)
			{
				var textCouldHaveWrapped = Element.LineBreakMode == LineBreakMode.WordWrap || Element.LineBreakMode == LineBreakMode.CharacterWrap;
				var textExceedsContainer = result.Request.Width > widthConstraint;

				if (textExceedsContainer || textCouldHaveWrapped)
				{
					var expandedWidth = Math.Max(tinyWidth, widthConstraint);
					result.Request = new Size(expandedWidth, result.Request.Height);
				}
			}

			return result;
		}

		private void UpdateLineBreakMode()
		{
#if __MOBILE__
			switch (Element.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					Control.TextContainer.LineBreakMode = UILineBreakMode.Clip;
					break;
				case LineBreakMode.WordWrap:
					Control.TextContainer.LineBreakMode = UILineBreakMode.WordWrap;
					break;
				case LineBreakMode.CharacterWrap:
					Control.TextContainer.LineBreakMode = UILineBreakMode.CharacterWrap;
					break;
				case LineBreakMode.HeadTruncation:
					Control.TextContainer.LineBreakMode = UILineBreakMode.HeadTruncation;
					break;
				case LineBreakMode.MiddleTruncation:
					Control.TextContainer.LineBreakMode = UILineBreakMode.MiddleTruncation;
					break;
				case LineBreakMode.TailTruncation:
					Control.TextContainer.LineBreakMode = UILineBreakMode.TailTruncation;
					break;
			}
#else
			switch (Element.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					Control.TextContainer.LineBreakMode = NSLineBreakMode.Clipping;
					break;
				case LineBreakMode.WordWrap:
					Control.TextContainer.LineBreakMode = NSLineBreakMode.ByWordWrapping;
					break;
				case LineBreakMode.CharacterWrap:
					Control.TextContainer.LineBreakMode = NSLineBreakMode.CharWrapping;
					break;
				case LineBreakMode.HeadTruncation:
					Control.TextContainer.LineBreakMode = NSLineBreakMode.TruncatingHead;
					break;
				case LineBreakMode.MiddleTruncation:
					Control.TextContainer.LineBreakMode = NSLineBreakMode.TruncatingMiddle;
					break;
				case LineBreakMode.TailTruncation:
					Control.TextContainer.LineBreakMode = NSLineBreakMode.TruncatingTail;
					break;
			}
#endif
		}

        private void UpdatePadding()
		{
            if (Element.Padding.IsEmpty)
            {
                return;
            }

#if __MOBILE__
			Control.TextContainerInset = new UIEdgeInsets(
					(float)Element.Padding.Top,
					(float)Element.Padding.Left,
					(float)Element.Padding.Bottom,
					(float)Element.Padding.Right);
			UpdateLayout();
#endif
		}

        private void UpdateLayout()
		{
#if __MOBILE__
			LayoutSubviews();
#else
			Layout();
#endif
		}

        private void UpdateTextDecorations()
		{
			if (Element?.TextType != TextType.Text)
				return;
#if __MOBILE__
			if (!(Control.AttributedText?.Length > 0))
				return;
#else
			if (!(Control.AttributedStringValue?.Length > 0))
				return;
#endif

			var textDecorations = Element.TextDecorations;
#if __MOBILE__
			var newAttributedText = new NSMutableAttributedString(Control.AttributedText);
			var strikeThroughStyleKey = UIStringAttributeKey.StrikethroughStyle;
			var underlineStyleKey = UIStringAttributeKey.UnderlineStyle;

#else
			var newAttributedText = new NSMutableAttributedString(Control.AttributedStringValue);
			var strikeThroughStyleKey = NSStringAttributeKey.StrikethroughStyle;
			var underlineStyleKey = NSStringAttributeKey.UnderlineStyle;
#endif
			var range = new NSRange(0, newAttributedText.Length);

			if ((textDecorations & TextDecorations.Strikethrough) == 0)
				newAttributedText.RemoveAttribute(strikeThroughStyleKey, range);
			else
				newAttributedText.AddAttribute(strikeThroughStyleKey, NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);

			if ((textDecorations & TextDecorations.Underline) == 0)
				newAttributedText.RemoveAttribute(underlineStyleKey, range);
			else
				newAttributedText.AddAttribute(underlineStyleKey, NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);

#if __MOBILE__
			Control.AttributedText = newAttributedText;
#else
			Control.AttributedStringValue = newAttributedText;
#endif
			UpdateCharacterSpacing();
			_perfectSizeValid = false;
		}

        private void UpdateCharacterSpacing()
		{

			if (Element?.TextType != TextType.Text)
				return;
#if __MOBILE__
			var textAttr = Control.AttributedText.AddCharacterSpacing(Element.Text, Element.CharacterSpacing);

			if (textAttr != null)
				Control.AttributedText = textAttr;
#else
   			var textAttr = Control.AttributedStringValue.AddCharacterSpacing(Element.Text, Element.CharacterSpacing);

			if (textAttr != null)
				Control.AttributedStringValue = textAttr;
#endif

			_perfectSizeValid = false;
		}

        private void UpdateHorizontalTextAlignment()
		{
#if __MOBILE__

			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)Element).EffectiveFlowDirection);
#else
			Control.Alignment = Element.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)Element).EffectiveFlowDirection);
#endif
		}
    }
}
