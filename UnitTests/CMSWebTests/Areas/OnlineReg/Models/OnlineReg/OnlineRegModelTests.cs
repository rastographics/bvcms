using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmsWeb.Areas.OnlineReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace CmsWeb.Areas.OnlineReg.Models.Tests
{
    public class OnlineRegModelTests
    {
        [Theory]
        [InlineData("Some One", @"<table><tbody><tr><td><registrant>Some One</registrant><span> is registered for an event<span><br></td></tr></tbody></table>")]
        [InlineData("Some One", @"<table><tbody><tr><td><registrant>Some One</registrant><span> is registered for an event<span><br/></td></tr></tbody></table>")]
        public void ValidateEmailRecipientRegistrantTest(string name, string detail)
        {
            var result = OnlineRegModel.ValidateEmailRecipientRegistrant(name, detail);
            result.ShouldBeTrue();
        }
    }
}
