using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Essentials;

namespace LabelHtml.Forms.Plugin.Abstractions
{
    public static class UriExtensions
    {
        public static bool IsHttp(this Uri uri) => uri != null && uri.Scheme.ToUpperInvariant().Contains("HTTP");
        public static bool IsEmail(this Uri uri) => uri.MatchSchema("mailto");
        public static bool IsTel(this Uri uri) => uri.MatchSchema("tel");
        public static bool IsSms(this Uri uri) => uri.MatchSchema("sms");
        public static bool IsGeo(this Uri uri) => uri.MatchSchema("geo");

        public static void LaunchBrowser(this Uri uri, BrowserLaunchOptions options)
        {
            if (options == null)
            {
                Browser.OpenAsync(uri);
            }
            else
            {
                Browser.OpenAsync(uri, options);
            }
        }

        public static void LaunchEmail(this Uri uri)
        {
            if (uri == null)
                return;

            var qParams = uri.ParseQueryString();
            var to = uri.Target();
            try
            {
                var message = new EmailMessage
                {
                    To = new List<string> { to },
                    Subject = qParams.GetFirst("subject"),
                    Body = qParams.GetFirst("body"),
                    Cc = qParams.Get("cc"),
                    Bcc = qParams.Get("bcc")
                };
                Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }

        public static void LaunchTel(this Uri uri)
        {
            if (uri == null)
                return;

            var to = uri.Target();
            try
            {
                PhoneDialer.Open(to);
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }

        public static void LaunchSms(this Uri uri)
        {
            if (uri == null)
                return;

            var qParams = uri.ParseQueryString();
            var to = uri.Target();
            try
            {
                var messageText = qParams.GetFirst("body");
                var message = new SmsMessage(messageText, new[] { to });
                Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }

        public static void LaunchMaps(this Uri uri)
        {
            if (uri == null)
                return;

            var target = uri.Target();
            try
            {
                var coordinates = target.Split(',');
                var latitude = double.Parse(coordinates[0], CultureInfo.InvariantCulture.NumberFormat);
                var longitude = double.Parse(coordinates[1].Split(';')[0], CultureInfo.InvariantCulture.NumberFormat);
                var location = new Location(latitude, longitude);
                Map.OpenAsync(location);
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }

        private static string Target(this Uri uri)
        {
            return Uri.UnescapeDataString(uri.AbsoluteUri.Substring(uri.Scheme.Length + 1).Split('?')[0]);
        }

        private static bool MatchSchema(this Uri uri, string schema)
        {
            return uri != null && uri.Scheme.Equals(schema, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
