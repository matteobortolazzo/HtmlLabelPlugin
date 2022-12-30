using System;
namespace LabelHtml.Forms.Plugin.Abstractions
{
    public class LinkEventArgs
    {
        public LinkEventArgs(string link) { Link = link; }
        public string Link { get; }
    }
}

