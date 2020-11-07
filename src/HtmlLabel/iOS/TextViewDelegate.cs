using Foundation;
using LabelHtml.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace LabelHtml.Forms.Plugin.iOS
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
