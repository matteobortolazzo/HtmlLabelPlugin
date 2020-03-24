using Xamarin.Forms;

using SharedHtmlLabel = LabelHtml.Forms.Plugin.Abstractions.HtmlLabel;

namespace LabelHtml.Forms.Plugin.PlatformSpecifics.Android
{
    public static class HtmlLabel
    {
        public static readonly BindableProperty HtmlLegacyModeEnabledProperty = BindableProperty.Create("HtmlLegacyModeEnabled", typeof(bool), typeof(SharedHtmlLabel), false, propertyChanged: OnHtmlLegacyModeEnabledPropertyChanged);

        private static void OnHtmlLegacyModeEnabledPropertyChanged(BindableObject element, object oldValue, object newValue)
        {
            var label = element as SharedHtmlLabel;
            label.HtmlLegacyModeEnabled = (bool)newValue;
        }

        public static bool GetHtmlLegacyModeEnabled(BindableObject element)
        {
            return (bool)element.GetValue(HtmlLegacyModeEnabledProperty);
        }

        public static void SetHtmlLegacyModeEnabled(BindableObject element, bool value)
        {
            element.SetValue(HtmlLegacyModeEnabledProperty, value);
        }

        public static bool GetHtmlLegacyModeEnabled(this IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, Label> config)
        {
            return GetHtmlLegacyModeEnabled(config.Element);
        }

        public static IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, Label> SetHtmlLegacyModeEnabled(this IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, Label> config, bool value)
        {
            SetHtmlLegacyModeEnabled(config.Element, value);
            return config;
        }
    }
}
