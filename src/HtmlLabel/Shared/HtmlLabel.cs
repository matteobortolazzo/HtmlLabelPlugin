using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("HtmlLabel.Forms.Plugin.Shared.Tests")]
namespace LabelHtml.Forms.Plugin.Abstractions
{
	/// <inheritdoc />
	/// <summary>
	/// A label that is able to display HTML content
	/// </summary>
	public class HtmlLabel : Label
	{
		/// <summary>
		/// Set the value of the MaxLines property
		/// </summary>
		/// <param name="view"></param>
		/// <param name="value"></param>
		public static void SetMaxLines(BindableObject view, int value)
		{
			view?.SetValue(MaxLinesProperty, value);
		}

		/// <summary>
		/// Send the Navigating event
		/// </summary>
		/// <param name="args"></param>
		internal void SendNavigating(WebNavigatingEventArgs args)
		{
			Navigating?.Invoke(this, args);
		}

		/// <summary>
		/// Send the Navigated event
		/// </summary>
		/// <param name="args"></param>
		internal void SendNavigated(WebNavigatingEventArgs args)
	    {
		    Navigated?.Invoke(this, args);
	    }

		/// <summary>
		/// Fires before the open URL request is done.
		/// </summary>
		public event EventHandler<WebNavigatingEventArgs> Navigating;

		/// <summary>
		/// Fires when the open URL request is done.
		/// </summary>
		public event EventHandler<WebNavigatingEventArgs> Navigated;
	}
}