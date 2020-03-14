using Windows.UI.Xaml;
using LabelHtml.Forms.Plugin.Abstractions;
using Xamarin.Forms.Platform.UWP;
using LabelHtml.Forms.Plugin.UWP;
using Microsoft.Xaml.Interactivity;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.UWP
{
    internal abstract class Behavior : DependencyObject, IBehavior
	{
		public void Attach(DependencyObject associatedObject)
		{
			AssociatedObject = associatedObject;
			OnAttached();
		}

		public void Detach() => OnDetaching();

		protected virtual void OnAttached() { }

		protected virtual void OnDetaching() { }

		protected DependencyObject AssociatedObject { get; set; }

		DependencyObject IBehavior.AssociatedObject => AssociatedObject;
	}
}