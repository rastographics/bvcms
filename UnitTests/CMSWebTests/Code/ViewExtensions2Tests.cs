using CmsWeb;
using Shouldly;
using System.Collections.Specialized;
using Xunit;

namespace CmsWebTests
{
    public class ViewExtensions2Tests
    {
        [Theory]
        [InlineData("name=John", "name", "John")]
        [InlineData("name=John&rating=5", "name", "John", "rating", "5")]
        [InlineData("email=user%40example.com&oauth+token=!V%23%5eW%26%24%25VUe%2f58r67n+90m8764c3-54vu%25%26B%5cIe",
                    "email", "user@example.com", "oauth token", "!V#^W&$%VUe/58r67n 90m8764c3-54vu%&B\\Ie")]
        public void NameValueCollectionToQueryStringTest(string expectedValue, params string[] keyValuePairs)
        {
            var form = new NameValueCollection();
            for (int i = 0; i < keyValuePairs.Length; i++)
            {
                form.Add(keyValuePairs[i++], keyValuePairs[i]);
            }
            form.ToQueryString().ShouldBe(expectedValue);
        }
    }
}
