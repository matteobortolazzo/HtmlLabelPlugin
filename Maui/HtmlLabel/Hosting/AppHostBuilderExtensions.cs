using HyperTextLabel.Maui.Controls;
using HyperTextLabel.Maui.Handlers;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace HyperTextLabel.Maui.Hosting
{
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder ConfigureHyperTextLabel(this MauiAppBuilder builder, bool useCompatibilityRenderers = false)
        {
            return useCompatibilityRenderers ?
                builder.UseMauiCompatibility()
                       .ConfigureMauiHandlers(handlers => handlers.AddLibraryCompatibilityRenderers()) :
                builder.ConfigureMauiHandlers(handlers => handlers.AddLibraryHandlers());
        }

        private static IMauiHandlersCollection AddLibraryCompatibilityRenderers(this IMauiHandlersCollection handlers)
        {
#if __ANDROID__
            //handlers.AddCompatibilityRenderer(typeof(HtmlLabel), typeof(Droid.HtmlLabelRenderer));
#elif __IOS__
            //handlers.AddCompatibilityRenderer(typeof(HtmlLabel), typeof(iOS.HtmlLabelRenderer));
#elif WINDOWS10_0_17763_0_OR_GREATER
            //handlers.AddCompatibilityRenderer(typeof(HtmlLabel), typeof(UWP.HtmlLabelRenderer));
#endif
            return handlers;
        }

        private static IMauiHandlersCollection AddLibraryHandlers(this IMauiHandlersCollection handlers)
        {
            handlers.AddHandler<HtmlLabel, HtmlLabelHandler>();

            return handlers;
        }
    }
}
