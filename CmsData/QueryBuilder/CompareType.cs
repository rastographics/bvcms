using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace CmsData
{
    public enum CompareType
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        LessEqual,
        GreaterEqual,
        Contains,
        DoesNotContain,
        StartsWith,
        DoesNotStartWith,
        EndsWith,
        DoesNotEndWith,
        OneOf,
        NotOneOf,
        AllTrue,// (expr1 AND expr2)
        AnyTrue,// (expr1 OR expr2)
        AllFalse,// NOT (expr1 OR expr2) == (NOT expr1 AND NOT expr2)
        AnyFalse,// NOT (expr1 AND expr2) == (NOT expr1 OR NOT expr2)
        After,
        Before,
        BeforeOrSame,
        AfterOrSame,
        AnyValue,
        IsNull,
        IsNotNull,
    }
}
