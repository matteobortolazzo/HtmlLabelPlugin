using System.ComponentModel;
using LabelHtml.Forms.Plugin.Abstractions;
using Xamarin.Forms.Platform.UWP;
using LabelHtml.Forms.Plugin.UWP;
using Microsoft.Xaml.Interactivity;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace LabelHtml.Forms.Plugin.UWP
{
	/// <summary>
	/// HtmlLable Implementation
	/// </summary>
	[Xamarin.Forms.Internals.Preserve(AllMembers = true)]
	public class HtmlLabelRenderer : LabelRenderer
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Initialize() { }

		/// <inheritdoc />
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
            ProcessText();
			base.OnElementChanged(e);
		}

		/// <inheritdoc />
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e != null && RendererHelper.RequireProcess(e.PropertyName))
			{
				ProcessText();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		private void ProcessText()
		{
			if (Control == null || Element == null)
            {
                return;
            }

			// Gets the complete HTML string
			var styledHtml = new RendererHelper(Element, Control.Text).ToString();
			if (styledHtml == null)
			{
				return;
			}

			Control.Text = styledHtml;

			// Adds the HtmlTextBehavior because UWP's TextBlock
			// does not natively support HTML content
			var behavior = new HtmlTextBehavior((HtmlLabel)Element);
            BehaviorCollection behaviors = Interaction.GetBehaviors(Control);
			behaviors.Clear();
			behaviors.Add(behavior);
		}
	}
}