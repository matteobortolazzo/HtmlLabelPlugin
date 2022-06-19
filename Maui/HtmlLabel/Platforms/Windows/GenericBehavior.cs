using Microsoft.UI.Xaml;

namespace HyperTextLabel.Maui.Platforms.Windows
{
    internal abstract class Behavior<T> : Behavior where T : DependencyObject
    {
        protected new T AssociatedObject => base.AssociatedObject as T;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
            {
                throw new InvalidOperationException("AssociatedObject is not of the right type");
            }
        }
    }
}