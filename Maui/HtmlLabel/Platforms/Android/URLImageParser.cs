using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Widget;
using Java.Net;

namespace HyperTextLabel.Maui.Platform.Droid
{
    internal class UrlDrawable : BitmapDrawable
    {
        public Drawable Drawable { get; set; }

        public override void Draw(Canvas canvas)
        {
            if (Drawable != null)
            {
                Drawable.Draw(canvas); ;
            }
        }
    }

    internal class ImageGetterAsyncTask : AsyncTask<string, int, Drawable>
    {
        private readonly UrlDrawable _urlDrawable;
        private readonly TextView _container;

        public ImageGetterAsyncTask(UrlDrawable urlDrawable, TextView container)
        {
            _urlDrawable = urlDrawable;
            _container = container;
        }

        protected override Drawable RunInBackground(params string[] @params)
        {
            var source = @params[0];
            return FetchDrawable(source);
        }

        protected override void OnPostExecute(Drawable result)
        {
            if (result == null)
            {
                return;
            }

            // Set the correct bound according to the result from HTTP call 
            _urlDrawable.SetBounds(0, 0, 0 + result.IntrinsicWidth, 0 + result.IntrinsicHeight);

            // Change the reference of the current drawable to the result from the HTTP call 
            _urlDrawable.Drawable = result;

            // Redraw the image by invalidating the container
            _container.Invalidate();

            // For ICS
            _container.SetHeight(_container.Height + result.IntrinsicHeight);

            // Pre ICS
            _container.Ellipsize = null;
        }

        private Drawable FetchDrawable(string urlString)
        {
            try
            {
                Stream stream = Fetch(urlString);
                var drawable = Drawable.CreateFromStream(stream, "src");
                drawable.SetBounds(0, 0, 0 + drawable.IntrinsicWidth, 0 + drawable.IntrinsicHeight);
                return drawable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
                return null;
            }
        }

        private static Stream Fetch(string urlString)
        {
            var url = new URL(urlString);
            var urlConnection = (HttpURLConnection)url.OpenConnection();
            Stream stream = urlConnection.InputStream;
            return stream;
        }
    }

    internal class UrlImageParser : Java.Lang.Object, Html.IImageGetter
    {
        private readonly TextView _container;

        public UrlImageParser(TextView container)
        {
            _container = container;
        }

        public Drawable GetDrawable(string source)
        {
            var urlDrawable = new UrlDrawable();

            var asyncTask = new ImageGetterAsyncTask(urlDrawable, _container);
            _ = asyncTask.Execute(source);

            return urlDrawable;
        }                
    }
}
