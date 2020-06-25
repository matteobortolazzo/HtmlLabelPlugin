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

        public static bool LaunchEmail(this Uri uri)
        {
            if (uri == null)
                return false;

            var qParams = uri.ParseQueryString();
            var to = uri.Target();
            try
            {
                var message = new EmailMessage
                {
                    To = new List<string> { to },
                    Subject = qParams.GetFirst("subject") ?? string.Empty,
                    Body = qParams.GetFirst("body") ?? string.Empty,
                    Cc = qParams.Get("cc") ?? new List<string>(),
                    Bcc = qParams.Get("bcc") ?? new List<string>()
                };
                Email.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
                return false;
            }

        }

        public static bool LaunchTel(this Uri uri)
        {
            if (uri == null)
                return false;

            var to = uri.Target();
            try
            {
                PhoneDialer.Open(to);
                return true;
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
                return false;
            }
        }

        public static bool LaunchSms(this Uri uri)
        {
            if (uri == null)
                return false;

            var qParams = uri.ParseQueryString();
            var to = uri.Target();
            try
            {
                var messageText = qParams.GetFirst("body");
                var message = new SmsMessage(messageText, new[] { to });
                Sms.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
                return false;
            }
        }

        public static bool LaunchMaps(this Uri uri)
        {
            if (uri == null)
                return false;

            var target = uri.Target();
            try
            {
                var coordinates = target.Split(',');
                var latitude = double.Parse(coordinates[0], CultureInfo.InvariantCulture.NumberFormat);
                var longitude = double.Parse(coordinates[1].Split(';')[0], CultureInfo.InvariantCulture.NumberFormat);
                var location = new Location(latitude, longitude);
                Map.OpenAsync(location);
                return true;
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
                return false;
            }
        }

        private static string Target(this Uri uri)
        {
            return Uri.UnescapeDataString(uri.AbsoluteUri.Substring(uri.Scheme.Length + 1).Split('?')[0].Replace("/", ""));
        }

        private static bool MatchSchema(this Uri uri, string schema)
        {
            return uri != null && uri.Scheme.Equals(schema, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
