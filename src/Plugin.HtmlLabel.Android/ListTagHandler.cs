using Java.Lang;
using System.Linq;
using Android.Text;
using Android.Text.Style;
using Org.Xml.Sax;

namespace Plugin.HtmlLabel.Android
{
    public class ListTagHandler : Object, Html.ITagHandler
    {
        private bool _first = true;
        private string _parent;
        private int _index = 1;

        public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
        {
            if (tag.Equals("ulc"))
            {
                _parent = "ulc";
                _index = 1;
            }
            else if (tag.Equals("olc"))
            {
                _parent = "olc";
                _index = 1;
            }

            if (!tag.Equals("lic")) return;

            var lastChar = (char) 0;
            if (output.Length() > 0)
            {
                lastChar = output.CharAt(output.Length() - 1);
            }
            if (_parent.Equals("ulc"))
            {
                if (_first)
                {
                    if (lastChar == '\n')
                        output.Append("\t•  ");
                    else
                        output.Append("\n\t•  ");
                    _first = false;
                }
                else
                    _first = true;
            }
            else
            {
                if (_first)
                {
                    if (lastChar == '\n')
                        output.Append("\t" + _index + ". ");
                    else
                        output.Append("\n\t" + _index + ". ");
                    _first = false;
                    _index++;
                }
                else
                    _first = true;
            }
        }
    }
}
