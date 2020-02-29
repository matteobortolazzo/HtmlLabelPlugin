using LabelHtml.Forms.Plugin.Droid;
using System;
using Xunit;

namespace HtmlLabel.Forms.Plugin.Android.Tests
{
    public class ListBuilderTests
    {
        [Fact]
        public void Check()
        {
            ListBuilder build = new ListBuilder();

            Assert.NotNull(build);
        }
    }
}
