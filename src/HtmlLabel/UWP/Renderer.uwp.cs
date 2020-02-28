using System.ComponentModel;
using LabelHtml.Forms.Plugin.Abstractions;
using Xamarin.Forms.Platform.UWP;
using LabelHtml.Forms.Plugin.UWP;
using Microsoft.Xaml.Interactivity;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
// ReSharper disable once CheckNamespace
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
            {
                return;
            }

            UpdateText();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e == null)
			{
				return;
			}

			if (e.PropertyName == Label.TextProperty.PropertyName ||
			         e.PropertyName == Label.FontAttributesProperty.PropertyName ||
			         e.PropertyName == Label.FontFamilyProperty.PropertyName ||
			         e.PropertyName == Label.FontSizeProperty.PropertyName ||
			         e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
			         e.PropertyName == Label.TextColorProperty.PropertyName)
            {
                UpdateText();
            }
        }

		private void UpdateText()
		{
			if (Control == null || Element == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Control.Text))
            {
                return;
            }

            // Gets the complete HTML string
            var helper = new RendererHelper(Element, Element.Text);
			Control.Text = helper.ToString();

			// Adds the HtmlTextBehavior because UWP's TextBlock
			// does not natively support HTML content
			var behavior = new HtmlTextBehavior((HtmlLabel)Element);
            BehaviorCollection behaviors = Interaction.GetBehaviors(Control);
			behaviors.Clear();
			behaviors.Add(behavior);
		}
	}
}