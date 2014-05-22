using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace CmsData
{
    public enum FieldType
    {
        Empty,
        String,
        Number,
        NumberLG,
        Integer,
        Date,
        DateSimple,
        Bit,
        NullBit,
        EqualBit,
        Code,
        NullCode,
        CodeStr,
        DateField,
        NullNumber,
        NullInteger,
        Group,
        StringEqual,
        StringEqualOrStartsWith,
        IntegerEqual,
        IntegerSimple,
    }
}
