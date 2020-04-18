using System;
using LabelHtml.Forms.Plugin.Abstractions;
using Xunit;

namespace LabelHtml.Forms.Plugin.Shared.Tests
{
    public class HttpUtilityTests
    {
        [Fact]
        public void ParseQueryString_WithNoParameters_ShouldReturn_NoEntries()
        {
            // Arrange
            var uri = new Uri("https://test.com?");

            // Act
            var r = uri.ParseQueryString();

            // Assert
            Assert.True(r.Count == 0);
        }

        [Fact]
        public void ParseQueryString_WithTwoParameters_ShouldReturn_TwoEntries()
        {
            // Arrange
            var uri = new Uri("https://test.com?a=1&b=2");

            // Act
            var r = uri.ParseQueryString();

            // Assert
            Assert.True(r.Count == 2);
        }

        [Fact]
        public void ParseQueryString_WithTwoEqualParameters_ShouldReturn_OneEntry()
        {
            // Arrange
            var uri = new Uri("https://test.com?a=1&a=2");

            // Act
            var r = uri.ParseQueryString();

            // Assert
            Assert.True(r.Count == 1);
            Assert.True(r["A"].Count == 2);
        }

        [Fact]
        public void ParseQueryString_WithOneParameterWithNoValue_ShouldReturn_OneEntry()
        {
            // Arrange
            var uri = new Uri("https://test.com?a");

            // Act
            var r = uri.ParseQueryString();

            // Assert
            Assert.True(r.Count == 1);
            Assert.True(r["A"].Count == 1);
        }

        [Fact]
        public void ParseQueryString_WithEncodedParameter_Should_Decode()
        {
            // Arrange
            var uri = new Uri("https://test.com?a=1%202");

            // Act
            var r = uri.ParseQueryString();

            // Assert
            Assert.True(r.Count == 1);
            Assert.True(r["A"][0] == "1 2");
        }

        [Fact]
        public void ParseQueryString_WithEncodedParameter_Should_NotDecode()
        {
            // Arrange
            var uri = new Uri("https://test.com?a=1%202");

            // Act
            var r = uri.ParseQueryString(false);

            // Assert
            Assert.True(r.Count == 1);
            Assert.True(r["A"][0] == "1%202");
        }
    }
}
