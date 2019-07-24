using Microsoft.VisualStudio.TestTools.UnitTesting;
using UtilityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shouldly;
using Xunit;

namespace UtilityExtensions.Tests
{
    public class UtilTests
    {
        [Fact]
        public void ToBoolTest()
        {
            object val = "";
            val.ToBool().ShouldBeFalse();

            val = false;
            val.ToBool().ShouldBeFalse();

            val = "false";
            val.ToBool().ShouldBeFalse();

            val = "False";
            val.ToBool().ShouldBeFalse();

            val = "FALSE";
            val.ToBool().ShouldBeFalse();

            val = "1";
            val.ToBool().ShouldBeFalse();

            val = null;
            val.ToBool().ShouldBeFalse();

            val = new { test = true };
            val.ToBool().ShouldBeFalse();

            val = true;
            val.ToBool().ShouldBeTrue();

            val = "true";
            val.ToBool().ShouldBeTrue();

            val = "True";
            val.ToBool().ShouldBeTrue();

            val = "TRUE";
            val.ToBool().ShouldBeTrue();

            val = new ToStringTrue();
            val.ToBool().ShouldBeTrue();
        }

        class ToStringTrue
        {
            public override string ToString() => "True";
        }
    }
}
