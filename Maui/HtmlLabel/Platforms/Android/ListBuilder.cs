using Android.Text;
using Android.Text.Style;
using Android.Widget;
using Java.Lang;

namespace LabelHtml.Forms.Plugin.Droid
{
    internal class ListBuilder
	{
		private int _listIndent = 20; // KWI-FIX : changed from constant to prop


		private readonly int _gap = 0;
		private readonly LiGap _liGap;
		private readonly ListBuilder _parent = null;

		private int _liIndex = -1;
		private int _liStart = -1;
		
		public ListBuilder(int listIndent) // KWI-FIX: added listIndent
		{
			_listIndent = listIndent;
			_parent = null;
			_gap = 0;
			_liGap = GetLiGap(null);
		}

		private ListBuilder(ListBuilder parent, bool ordered, int listIndent) // KWI-FIX: added listIndent
		{
			_listIndent = listIndent;
			_parent = parent;
			_liGap = parent._liGap;
			_gap = parent._gap + _listIndent + _liGap.GetGap(ordered);
			_liIndex = ordered ? 0 : -1;
		}

		public ListBuilder StartList(bool ordered, IEditable output)
		{
			if (_parent == null && output.Length() > 0)
			{
				_ = output.Append("\n ");
			}
			return new ListBuilder(this, ordered, _listIndent); // KWI-FIX: pass thru listIndent
		}

		public void AddListItem(bool isOpening, IEditable output)
		{
			if (isOpening)
			{
				EnsureParagraphBoundary(output);
				_liStart = output.Length();

				var lineStart = IsOrdered()
					? ++_liIndex + ". "
					: "•  ";
				_ = output.Append(lineStart);
			}
			else
			{
				if (_liStart >= 0)
				{
					EnsureParagraphBoundary(output);
					using var leadingMarginSpan = new LeadingMarginSpanStandard(_gap - _liGap.GetGap(IsOrdered()), _gap);
					output.SetSpan(leadingMarginSpan, _liStart, output.Length(), SpanTypes.ExclusiveExclusive);
					_liStart = -1;
				}
			}
		}

		public ListBuilder CloseList(IEditable output)
		{
			EnsureParagraphBoundary(output);
			ListBuilder result = _parent;
			if (result == null)
			{
				result = this;
			}

			if (result._parent == null)
			{
				_ = output.Append('\n');
			}

			return result;
		}

		private bool IsOrdered()
		{
			return _liIndex >= 0;
		}

		private static void EnsureParagraphBoundary(IEditable output)
		{
			if (output.Length() == 0)
			{
				return;
			}

			var lastChar = output.CharAt(output.Length() - 1);
			if (lastChar != '\n')
			{
				_ = output.Append('\n');
			}
		}

		private class LiGap
		{
			private readonly int _orderedGap;
			private readonly int _unorderedGap;

			internal LiGap(int orderedGap, int unorderedGap)
			{
				_orderedGap = orderedGap;
				_unorderedGap = unorderedGap;
			}

			public int GetGap(bool ordered)
			{
				return ordered ? _orderedGap : _unorderedGap;
			}
		}

		private static LiGap GetLiGap(TextView tv)
		{
			var orderedGap = tv == null ? 40 : ComputeWidth(tv, true);
			var unorderedGap = tv == null ? 30 : ComputeWidth(tv, false);

			return new LiGap(orderedGap, unorderedGap);
		}

		private static int ComputeWidth(TextView tv, bool isOrdered)
		{
			Android.Graphics.Paint paint = tv.Paint;
			using var bounds = new Android.Graphics.Rect();
			var startString = isOrdered ? "99. " : "• ";
		    paint.GetTextBounds(startString, 0, startString.Length, bounds);
			var width = bounds.Width();
			var pt = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Pt, width, tv.Context.Resources.DisplayMetrics);			
			return (int)pt;
		}
	}
}
