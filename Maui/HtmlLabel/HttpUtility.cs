namespace LabelHtml.Forms.Plugin.Abstractions
{
    public static class HttpUtility
    {
        public static Dictionary<string, List<string>> ParseQueryString(this Uri uri, bool decode = true)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uri.Query.Length == 0 || uri.Query.Length == 1 && uri.Query[0] == '?')
            {
                return new Dictionary<string, List<string>>();
            }

            var query = uri.Query;
            if (query[0] == '?')
            {
                query = query.Substring(1);
            }

            return query
                .Split('&')
                .Select(p => p.Split('='))
                .Select(p => p.Length == 1 ? (p[0], "true") : (p[0], p[1]))
                .GroupBy(p => p.Item1.ToUpperInvariant())
                .ToDictionary(
                    g => g.Key, 
                    g =>
                    {
                        var values = g.Select(p => p.Item2);
                        if (decode)
                            values = values.Select(Uri.UnescapeDataString);
                        return values.ToList();
                    });
        }

        public static string GetFirst(this Dictionary<string, List<string>> qParams, string key) =>
            qParams.Get(key)?.FirstOrDefault();

        public static List<string> Get(this Dictionary<string, List<string>> qParams, string key) =>
            qParams != null && key != null && qParams.ContainsKey(key.ToUpperInvariant()) ? qParams[key.ToUpperInvariant()] : null;
    }
}