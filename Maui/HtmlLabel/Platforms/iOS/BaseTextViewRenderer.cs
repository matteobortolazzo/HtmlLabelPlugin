using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using Foundation;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using LabelHtml.Forms.Plugin.Abstractions;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using System.Runtime.InteropServices;

#if __MOBILE__
using UIKit;
using NativeTextView = UIKit.UITextView;

#else
using AppKit;
using NativeTextView = AppKit.NSTextView;
#endif

#if __MOBILE__
namespace LabelHtml.Forms.Plugin.iOS
#else
namespace LabelHtml.Forms.Plugin.MacOS
#endif
{
    public abstract class BaseTextViewRenderer<TElement> : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<TElement, NativeTextView>
		where TElement : Label
	{
        private SizeRequest _perfectSize;
        private bool _perfectSizeValid;

        /// <inheritdoc />
        private RectangleF _frame;

        static HashSet<string> s_perfectSizeSet = new HashSet<string>
                                                  {
                                                      Label.TextProperty.PropertyName,
                                                      Label.TextColorProperty.PropertyName,
                                                      Label.FontAttributesProperty.PropertyName,
													  Label.FontFamilyProperty.PropertyName,
													  Label.FontSizeProperty.PropertyName,
													  Label.LineBreakModeProperty.PropertyName,
                                                      Label.HorizontalTextAlignmentProperty.PropertyName,
                                                      Label.LineHeightProperty.PropertyName,
                                                      Label.PaddingProperty.PropertyName,
                                                      HtmlLabel.LinkColorProperty.PropertyName
                                                  };

		protected override NativeTextView CreateNativeControl()
		{
#if __MOBILE__
			return //Element.Padding.IsEmpty ?
				new NativeTextView(RectangleF.Empty)
				{
					Editable = false,
					ScrollEnabled = false,
					ShowsVerticalScrollIndicator = false,
					BackgroundColor = UIColor.Clear
				};
    //            :
				//new FormsTextView(RectangleF.Empty)
				//{
				//	Editable = false,
				//	ScrollEnabled = false,
				//	ShowsVerticalScrollIndicator = false,
				//	BackgroundColor = UIColor.Clear
				//};
#else
            return new NativeTextView(RectangleF.Empty)
                   {
                       Editable = false,
                       ScrollEnabled = false,
                       ShowsVerticalScrollIndicator = false,
                       BackgroundColor = UIColor.Clear
                   };
#endif
		}

        protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			_perfectSizeValid = false;

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanging -= ElementPropertyChanging;

                //TODO? Unwind the GestureRecognizer code below?
			}

			if (e.NewElement != null)
			{
				//try
				//{
				    e.NewElement.PropertyChanging += ElementPropertyChanging;
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
							    // BUG: this captures the Element and the recognizer
							    Control.AddGestureRecognizer(
								    new UITapGestureRecognizer(
                                    () =>
									{
										ICommand cmd = recognizer.Command;
										if (cmd != null && cmd.CanExecute(recognizer.CommandParameter))
											cmd.Execute(recognizer.CommandParameter);
									})
								    { NumberOfTapsRequired = (nuint)recognizer.NumberOfTapsRequired });
						    }
					    }
				    }

                    UpdateLineBreakMode();
                    UpdateText();
                    UpdateTextColor();
                    UpdateTextDecorations();
                    UpdateMaxLines();
                    UpdateCharacterSpacing();
                    UpdatePadding();
                    UpdateHorizontalTextAlignment();
				//catch (System.Exception ex)
				//{
				//	System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
				//}
			}
			base.OnElementChanged(e);
		}

        void ElementPropertyChanging( object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e )
        {
            if ( s_perfectSizeSet.Contains( e.PropertyName ) )
                _perfectSizeValid = false;
        }

		protected abstract void UpdateText();
		protected abstract bool NavigateToUrl(NSUrl url);

        public override SizeRequest GetDesiredSize( double widthConstraint, double heightConstraint )
        {
            var retval = GetDesiredSizeInternal(widthConstraint, heightConstraint);
            Debug.WriteLine( $"SizeRequest = {retval}" );

            return retval;
		}

        public SizeRequest GetDesiredSizeInternal( double widthConstraint, double heightConstraint )
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

			bool containerIsNotInfinitelyWide = !double.IsInfinity(widthConstraint);

			if (containerIsNotInfinitelyWide)
			{
				bool textCouldHaveWrapped = Element.LineBreakMode == LineBreakMode.WordWrap || Element.LineBreakMode == LineBreakMode.CharacterWrap;
				bool textExceedsContainer = result.Request.Width > widthConstraint;

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
            this.Control.InvokeOnMainThread( () =>
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
		);
			this.UpdateLayout();
		}

        private void UpdatePadding()
		{
			if (Element == null || Control == null)
				return;

			//if (Element.Padding.IsEmpty)
			//	return;

#if __MOBILE__
			//var formsTextView = Control as FormsTextView;
			//if (formsTextView == null)
			//{
			//	Debug.WriteLine(
			//		$"{nameof(HtmlLabelRenderer)}: On iOS, a Label created with no padding will ignore padding changes");

			//	return;
			//}

			//formsTextView.TextInsets = new UIEdgeInsets(
			//	(float)Element.Padding.Top,
			//	(float)Element.Padding.Left,
			//	(float)Element.Padding.Bottom,
			//	(float)Element.Padding.Right);

            Control.TextContainerInset = new UIEdgeInsets(
                (float)Element.Padding.Top,
                (float)Element.Padding.Left,
                (float)Element.Padding.Bottom,
                (float)Element.Padding.Right );
            
			UpdateLayout();
#endif
		}
		
        private void UpdateLayout()
		{
#if __MOBILE__
			LayoutSubviews();
            this.Control.TextContainer.Size = this.Bounds.Size;
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
			if (Element == null || Control == null)
				return;

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

        protected override void OnElementPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            base.OnElementPropertyChanged( sender, e );
            if ( e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName )
                UpdateHorizontalTextAlignment();
            else if ( e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName )
                UpdateLayout();
            else if ( e.PropertyName == Label.TextColorProperty.PropertyName )
                UpdateTextColor();
            //else if ( e.PropertyName == Label.FontProperty.PropertyName )
            //    UpdateFont();
            else if ( e.PropertyName == Label.TextProperty.PropertyName )
            {
                UpdateText();
                UpdateTextColor();
                UpdateTextDecorations();
                UpdateCharacterSpacing();
            }
            else if (e.PropertyName == Label.CharacterSpacingProperty.PropertyName)
                UpdateCharacterSpacing();
            else if (e.PropertyName == Label.TextDecorationsProperty.PropertyName)
                UpdateTextDecorations();
            else if ( e.PropertyName == Label.FormattedTextProperty.PropertyName )
            {
                UpdateText();
                UpdateTextDecorations();
            }
            else if ( e.PropertyName == Label.LineBreakModeProperty.PropertyName )
                UpdateLineBreakMode();
            else if ( e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName )
                UpdateHorizontalTextAlignment();
            else if ( e.PropertyName == Label.LineHeightProperty.PropertyName )
            {
                UpdateText();
                UpdateTextDecorations();
            }
            else if ( e.PropertyName == Label.MaxLinesProperty.PropertyName )
                UpdateMaxLines();
            else if ( e.PropertyName == Label.PaddingProperty.PropertyName )
                UpdatePadding();
            else if ( e.PropertyName == Label.TextTypeProperty.PropertyName )
                UpdateText();
            else if ( e.PropertyName == Label.TextTransformProperty.PropertyName )
                UpdateText();
		}

		private void UpdateMaxLines()
        {
            this.Control.InvokeOnMainThread( () =>
                                             {
                                                 if ( Element.MaxLines >= 0 )
                                                 {
#if __MOBILE__
                                                     Control.TextContainer.MaximumNumberOfLines =
                                                         (nuint)this.Element.MaxLines;

#else
				Control.TextContainer.MaximumNumberOfLines = Element.MaxLines;

				Layout();
#endif
                                                 }
                                                 else
                                                 {
#if __MOBILE__
                                                     switch ( Element.LineBreakMode )
                                                     {
                                                         case LineBreakMode.WordWrap:
                                                         case LineBreakMode.CharacterWrap:
                                                             Control.TextContainer.MaximumNumberOfLines = 0;

                                                             break;
                                                         case LineBreakMode.NoWrap:
                                                         case LineBreakMode.HeadTruncation:
                                                         case LineBreakMode.MiddleTruncation:
                                                         case LineBreakMode.TailTruncation:
                                                             Control.TextContainer.MaximumNumberOfLines = 1;

                                                             break;
                                                     }

#else
				switch (Element.LineBreakMode)
				{
					case LineBreakMode.WordWrap:
					case LineBreakMode.CharacterWrap:
						Control.TextContainer.MaximumNumberOfLines = 0;
						break;
					case LineBreakMode.NoWrap:
					case LineBreakMode.HeadTruncation:
					case LineBreakMode.MiddleTruncation:
					case LineBreakMode.TailTruncation:
						Control.TextContainer.MaximumNumberOfLines = 1;
						break;
				}

				Layout();
#endif
                                                 }

                                                 LayoutSubviews();
                                             });

		}

		void UpdateTextColor()
		{
            if ( Element == null || Control == null )
				return;

            var textColor = Element.TextColor;

			if ( textColor.IsDefault() && textColor.ToUIColor().IsEqualToColor( Control.TextColor ))
			{
				// If no explicit text color has been specified let the HTML determine the colors
				return;
			}

			// default value of color documented to be black in iOS docs
#if __MOBILE__
            Control.TextColor = textColor.ToUIColor( ColorExtensions.LabelColor );
#else
			var alignment = Element.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)Element).EffectiveFlowDirection);
			var textWithColor = new NSAttributedString(Element.Text ?? "", font: Element.ToNSFont(), foregroundColor: textColor.ToNSColor(ColorExtensions.TextColor), paragraphStyle: new NSMutableParagraphStyle() { Alignment = alignment });
			textWithColor = textWithColor.AddCharacterSpacing(Element.Text ?? string.Empty, Element.CharacterSpacing);
			Control.AttributedStringValue = textWithColor;
#endif
			UpdateLayout();
		}

#if __MOBILE__
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
#else
		public override void Layout()
		{
			base.Layout();
#endif

			if (Control == null)
				return;

			SizeF fitSize;
			NFloat labelHeight;
			switch (Element.VerticalTextAlignment)
			{
				case TextAlignment.Start:
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (NFloat)Math.Min(Bounds.Height, fitSize.Height);
					Control.Frame = new RectangleF(0, 0, (NFloat)Element.Width, labelHeight);
					Debug.WriteLine( $"LayoutSubviews = {this.Control.Frame}" );
					break;
				case TextAlignment.Center:

#if __MOBILE__
					Control.Frame = new RectangleF(0, 0, (NFloat)Element.Width, (NFloat)Element.Height);
                    Debug.WriteLine( $"LayoutSubviews = {this.Control.Frame}" );
#else
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (NFloat)Math.Min(Bounds.Height, fitSize.Height);
					var yOffset = (int)(Element.Height / 2 - labelHeight / 2);
					Control.Frame = new RectangleF(0, 0, (NFloat)Element.Width, (NFloat)Element.Height - yOffset);
#endif
					break;
				case TextAlignment.End:
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (NFloat)Math.Min(Bounds.Height, fitSize.Height);
#if __MOBILE__
					NFloat yOffset = 0;
					yOffset = (NFloat)(Element.Height - labelHeight);
					Control.Frame = new RectangleF(0, yOffset, (NFloat)Element.Width, labelHeight);
                    Debug.WriteLine( $"LayoutSubviews = {this.Control.Frame}" );
#else
					Control.Frame = new RectangleF(0, 0, (NFloat)Element.Width, labelHeight);
#endif
					break;
			}
		}

//#if __MOBILE__

//        protected override void SetAccessibilityLabel()
//        {
//            // If we have not specified an AccessibilityLabel and the AccessibiltyLabel is current bound to the Text,
//            // exit this method so we don't set the AccessibilityLabel value and break the binding.
//            // This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
//            // will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Text 
//            // of the Label.

//            var elemValue = (string)Element?.GetValue( AutomationProperties.NameProperty );

//            if ( string.IsNullOrWhiteSpace( elemValue ) && Control?.AccessibilityLabel == Control?.Text )
//                return;

//            base.SetAccessibilityLabel();
//        }
//#endif

        protected override void SetBackgroundColor( Color? color )
        {
#if __MOBILE__
            if ( color == KnownColor.Default)
                BackgroundColor = UIColor.Clear;
            else
                BackgroundColor = color.ToUIColor();
#else
			if (color == Color.Default)
				Layer.BackgroundColor = NSColor.Clear.CGColor;
			else
				Layer.BackgroundColor = color.ToCGColor();
#endif
        }

        protected override void SetBackground( Brush brush )
        {
            var backgroundLayer = this.GetBackgroundLayer( brush );

            if ( backgroundLayer != null )
            {
#if __MOBILE__
                Layer.BackgroundColor = UIColor.Clear.CGColor;
#endif
                Layer.InsertBackgroundLayer( backgroundLayer, 0 );
            }
            else
                Layer.RemoveBackgroundLayer();
        }
	}

#if __MOBILE__
	class FormsTextView : NativeTextView
	{
		public UIEdgeInsets TextInsets { get; set; }

		public FormsTextView(RectangleF frame) : base(frame)
		{
		}

        public override void Draw(RectangleF rect) => base.Draw(TextInsets.InsetRect(rect));

		public override SizeF SizeThatFits(SizeF size) => AddInsets(base.SizeThatFits(size));

		SizeF AddInsets(SizeF size) => new SizeF(
			width: size.Width + TextInsets.Left + TextInsets.Right,
			height: size.Height + TextInsets.Top + TextInsets.Bottom);
	}
#endif
}
