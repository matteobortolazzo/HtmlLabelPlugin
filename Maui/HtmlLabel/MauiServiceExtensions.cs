namespace LabelHtml.Forms.Plugin.Abstractions
{
    /// <summary>
    /// These extension methods were pulled in from MAUI codebase as they are not public.
    /// </summary>
    internal static class MauiServiceExtensions
    {
        public static IServiceProvider GetServiceProvider(this IElementHandler handler)
        {
            var context = handler.MauiContext ??
                throw new InvalidOperationException($"Unable to find the context. The {nameof(handler.MauiContext)} property should have been set by the host.");

            var services = context?.Services ??
                throw new InvalidOperationException($"Unable to find the service provider. The {nameof(handler.MauiContext)} property should have been set by the host.");

            return services;
        }

        public static T GetRequiredService<T>(this IElementHandler handler)
            where T : notnull
        {
            var services = handler.GetServiceProvider();

            var service = services.GetRequiredService<T>();

            return service;
        }

    }
}
