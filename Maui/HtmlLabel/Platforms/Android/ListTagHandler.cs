using Android.Text;
using Org.Xml.Sax;

namespace LabelHtml.Forms.Plugin.Droid
{
    /// <summary>
    /// Tag handler to support HTML lists.
    /// </summary>
    internal class ListTagHandler : Java.Lang.Object, Html.ITagHandler
	{
		public const string TagUl = "ULC";
		public const string TagOl = "OLC";
		public const string TagLi = "LIC";

		private ListBuilder _listBuilder; // KWI-FIX: removed new, set in constructor
		public ListTagHandler(int listIndent) // KWI-FIX: added constructor with listIndent property
		{
			_listBuilder = new ListBuilder(listIndent);
		}

		public void HandleTag(bool isOpening, string tag, IEditable output, IXMLReader xmlReader)
		{
			tag = tag.ToUpperInvariant();
			var isItem = tag == TagLi;

			// Is list item
			if (isItem)
			{
				_listBuilder.AddListItem(isOpening, output);
			}
			// Is list
			else
			{
				if (isOpening)
				{
					var isOrdered = tag == TagOl;
					_listBuilder = _listBuilder.StartList(isOrdered, output);
				}
				else
				{
					_listBuilder = _listBuilder.CloseList(output);
				}
			}			
		}
	}
}
