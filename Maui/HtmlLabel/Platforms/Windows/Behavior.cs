﻿using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace HyperTextLabel.Maui.Platforms.Windows
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