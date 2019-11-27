using System.Linq;
using CmsWeb.Models;
using Shouldly;
using Xunit;

namespace CMSWebTests.Models
{
    public class IDbBinderTests
    {
        //This test enforces a parameterless constructor on all classes that implement IDbBinder
        [Fact]
        public void IDbBinder_Implements_Parameterless_Constructor()
        {
            var idbbinder = typeof(IDbBinder);
            var assembly = idbbinder.Assembly;
            var classes = assembly.GetTypes().Where(t => t.IsClass && idbbinder.IsAssignableFrom(t));
            foreach(var c in classes)
            {
                c.GetConstructors()
                    .Where(ctor => ctor.GetParameters().Length == 0)
                    .Count().ShouldBe(1, customMessage: $"{c} did not implement a parameterless constructor");
            }
        }
    }
}
