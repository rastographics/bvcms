//------------------------------------------------------------------------------
// <copyright file="CSSqlFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    [SqlFunction]
    public static SqlBoolean IsValidEmail(SqlString addr)
    {
        if (addr == null)
            return true;
        var email = addr.Value.Trim();
        if (string.IsNullOrEmpty(email))
            return true;
        var re1 = new Regex(@"^(.*\b(?=\w))\b[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9._-]+\.[A-Z]{2,4}\b\b(?!\w)$", RegexOptions.IgnoreCase);
        var re2 = new Regex(@"^[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
        return re1.IsMatch(email) || re2.IsMatch(email);
    }
    [SqlFunction]
    public static SqlString RegexMatch(SqlString subject, SqlString pattern)
    {
        try
        {
            return Regex.Match(subject.Value, pattern.Value, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled).Value;
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send("Error searching Pattern " + ex.Message);
            return "";
        }
    }
    [SqlFunction]
    public static SqlString RegexMatchGroup(SqlString subject, SqlString pattern, SqlString group)
    {
        try
        {
            var m = Regex.Match(subject.Value, pattern.Value, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return m.Groups[group.Value].Value;
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send("Error searching Pattern " + ex.Message);
            return "";
        }
    }
    [SqlFunction]
    public static SqlString AllRegexMatchs(SqlString subject, SqlString pattern)
    {
        try
        {
            var list = new List<string>();
            var re = new Regex(pattern.Value);
            var match = re.Match(subject.Value);
            while (match.Success)
            {
                list.Add(match.Value);
                match = match.NextMatch();
            }
            return string.Join("<br>\n", list.ToArray());
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send("Error searching Pattern " + ex.Message);
            return "";
        }
    }
}
