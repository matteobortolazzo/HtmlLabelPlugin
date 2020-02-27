using LabelHtml.Forms.Plugin.Abstractions;
using System;
using Xamarin.Forms;
using Xunit;

namespace LabelHtml.Forms.Plugin.Shared.Tests
{
    public class HtmlLabelTests
    {
        [Fact]
        public void OnSendNavigating_Navigating_IsInvoked()
        {
            // Arrange
            var label = new HtmlLabel();
            var url = Guid.NewGuid().ToString();
            var source = new UrlWebViewSource();
            var expectedArgs = new WebNavigatingEventArgs(WebNavigationEvent.Forward, source, url);
            label.Navigating += LabelNavigating;

            WebNavigatingEventArgs actualArgs = null;
            object actualSender = null;

            void LabelNavigating(object sender, WebNavigatingEventArgs e)
            {
                actualSender = sender;
                actualArgs = e;
            }

            // Act
            label.SendNavigating(expectedArgs);

            // Assert
            Assert.Equal(expectedArgs, actualArgs);
            Assert.Equal(label, actualSender);
        }

        [Fact]
        public void OnSendNavigated_Navigated_IsInvoked()
        {
            // Arrange
            var label = new HtmlLabel();
            var url = Guid.NewGuid().ToString();
            var source = new UrlWebViewSource();
            var expectedArgs = new WebNavigatingEventArgs(WebNavigationEvent.Forward, source, url);
            label.Navigated += LabelNavigated;

            WebNavigatingEventArgs actualArgs = null;
            object actualSender = null;

            void LabelNavigated(object sender, WebNavigatingEventArgs e)
            {
                actualSender = sender;
                actualArgs = e;
            }

            // Act
            label.SendNavigated(expectedArgs);

            // Assert
            Assert.Equal(expectedArgs, actualArgs);
            Assert.Equal(label, actualSender);
        }
    }
}
