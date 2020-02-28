using System;
using Android.Text;
using LabelHtml.Forms.Plugin.Abstractions;
using LabelHtml.Forms.Plugin.Droid;
using Org.Xml.Sax;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.Droid
{
    // TagHandler that handles lists (ul, ol)
    internal class ListTagHandler : Java.Lang.Object, Html.ITagHandler
	{
		public const string TagUlc = "ULC";
		public const string TagOlc = "OLC";
		public const string TagLic = "LIC";

		private ListBuilder _listBuilder = new ListBuilder();
		
		public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
		{
			tag = tag.ToUpperInvariant();
			if (tag.Equals(TagLic, StringComparison.Ordinal))
			{
				_listBuilder.Li(opening, output);
				return;
			}
			if (opening)
			{
				if (tag.Equals(TagOlc, StringComparison.Ordinal))
				{
					_listBuilder = _listBuilder.StartList(true, output);
					return;
				}
				if (tag.Equals(TagUlc, StringComparison.Ordinal))
				{
					_listBuilder = _listBuilder.StartList(false, output);
					return;
				}
			}
			else
			{
				if (tag.Equals(TagOlc, StringComparison.Ordinal) || tag.Equals(TagUlc, StringComparison.Ordinal))
				{
					_listBuilder = _listBuilder.CloseList(output);
				}
				return;
			}
		}
	}
}
