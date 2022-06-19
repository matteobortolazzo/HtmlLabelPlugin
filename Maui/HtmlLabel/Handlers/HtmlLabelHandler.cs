using HyperTextLabel.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace HyperTextLabel.Maui.Handlers
{
    public partial class HtmlLabelHandler
    {
        public static IPropertyMapper<IHtmlLabel, HtmlLabelHandler> Mapper =
            new PropertyMapper<HtmlLabel, HtmlLabelHandler>(LabelHandler.Mapper)
            {
                [nameof(ILabel.Text)] = MapLabelText,
                [nameof(Label.FormattedText)] = MapLabelText,
                [nameof(Label.TextTransform)] = MapLabelText,
                [nameof(Label.TextType)] = MapLabelText,
                [nameof(IHtmlLabel.UnderlineText)] = MapUnderlineText,
                [nameof(IHtmlLabel.LinkColor)] = MapLinkColor,
                [nameof(IHtmlLabel.BrowserLaunchOptions)] = MapBrowserLaunchOptions,
                [nameof(IHtmlLabel.AndroidLegacyMode)] = MapAndroidLegacyMode,
                [nameof(IHtmlLabel.AndroidListIndent)] = MapAndroidListIndent,
            };

        public HtmlLabelHandler() : this(null)
        {

        }

        public HtmlLabelHandler(IPropertyMapper mapper = null) : base(mapper ?? Mapper)
        {

        }
    }
}
