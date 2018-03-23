using Java.Lang;
using System.Linq;
using Android.Text;
using Android.Text.Style;
using Org.Xml.Sax;

namespace Plugin.HtmlLabel.Android
{
    public class ListTagHandler : Object, Html.ITagHandler 
    {
        bool first = true;
        string parent = null;
        int index = 1;

        public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
        {
            if (tag.Equals("ul")) parent = "ul";
            else if (tag.Equals("ol")) parent = "ol";
            
            if (tag.Equals("li"))
            {
                char lastChar = '0';
                if (output.Length() > 0)
                    lastChar = output.CharAt(output.Length() - 1);
                if (parent.Equals("ul"))
                {
                    if (first)
                    {
                        if (lastChar == '\n')
                            output.Append("\t•  ");
                        else
                            output.Append("\n\t•  ");
                        first = false;
                    }
                    else
                    {
                        first = true;
                    }
                }
                else
                {
                    if (first)
                    {
                        if (lastChar == '\n')
                            output.Append("\t" + index + ". ");
                        else
                            output.Append("\n\t"+ index + ". ");
                        first = false;
                        index++;
                    }
                    else
                    {
                        first = true;
                    }
                }
            }
        }
    }
}
