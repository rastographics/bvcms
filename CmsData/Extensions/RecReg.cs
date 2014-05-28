using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
    public partial class RecReg
    {
        public void AddToComments(string s)
        {
            Comments = s + "\n" + Comments;
        }
    }
}
