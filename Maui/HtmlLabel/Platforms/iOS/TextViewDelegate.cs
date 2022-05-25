using Foundation;
using UIKit;

namespace HyperTextLabel.Maui.Platforms.iOS
{
    internal class TextViewDelegate : UITextViewDelegate
	{
		private Func<NSUrl, bool> _navigateTo;

		public TextViewDelegate(Func<NSUrl, bool> navigateTo)
		{
			_navigateTo = navigateTo;
		}

		public override bool ShouldInteractWithUrl(UITextView textView, NSUrl URL, NSRange characterRange)
		{
			if (_navigateTo != null)
			{
				return _navigateTo(URL);
			}
			return true;
		}
	}

}
