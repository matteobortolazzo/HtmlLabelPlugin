using CoreGraphics;
using UIKit;

namespace LabelHtml.Forms.Plugin.iOS
{
    public class UITextViewFixedWithKludge : UITextView
    {
        public UITextViewFixedWithKludge(CGRect frame) : base(frame)
        {
            
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Setup();
        }

        private void Setup()
        {
            TextContainerInset = UIEdgeInsets.Zero;
            TextContainer.LineFragmentPadding = 0;

            var b = Bounds;
            var h = SizeThatFits(new CGSize(
                Bounds.Size.Width,
                float.MaxValue)).Height;
            Bounds = new CGRect(b.X, b.Y, b.Width, h);
        }
    }
}
