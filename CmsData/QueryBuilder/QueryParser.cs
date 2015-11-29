using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.QueryBuilder;
using UtilityExtensions;

namespace CmsData
{
    public class QueryParser
    {
        private readonly QueryLexer lexer;
        private Token token;

        private QueryParser(string s)
        {
            lexer = new QueryLexer(s);
        }
        public static Condition Parse(string s)
        {
            var m = new QueryParser(s);
            return m.ParseCondition();
        }
        private Condition ParseCondition(Condition p = null)
        {
            // if no surrounding group, create one
            if (p == null && token.Type != TokenType.LParen && token.Type != TokenType.Not)
            {
                p = Condition.CreateAllGroup();
                return AddConditions(p);
            }
            var allClauses = p == null ? new Dictionary<Guid, Condition>() : p.AllConditions;
            Guid? parentGuid = null;
            if (p != null)
                parentGuid = p.Id;

            var c = new Condition
            {
                ParentId = parentGuid,
                Id = Guid.NewGuid(),
                AllConditions = allClauses
            };
            switch (token.Type)
            {
                case TokenType.Not:
                    token = lexer.Next(); // skip to LParen
                    if (token.Type != TokenType.LParen)
                        throw new Exception("Missing '(' after NOT");
                    token = lexer.Next(); // skip to condition list
                    c.ConditionName = "Group";
                    c.Comparison = CompareType.AllFalse.ToString();
                    return AddConditions(c);
                case TokenType.LParen:
                    token = lexer.Next(); // skip to first condition in group
                    c.ConditionName = "Group";
                    return AddConditions(c);
                case TokenType.Name:
                    c.ConditionName = token.Text;
                    break;
                case TokenType.Func:
                    c.ConditionName = token.Text;
                    token = lexer.Next(); // skip to (
                    token = lexer.Next(); // skip to first param
                    do
                        ParseParam(c);
                    while (token.Type == TokenType.Comma);
                    if (token.Type != TokenType.RParen)
                        throw new Exception("missing ) on function parameters");
                    break;
            }
            var op = lexer.Next(); // get operator
            if (op.Type != TokenType.Op)
                throw new Exception("expected comparision operator");
            token = lexer.Next(); // skip to rightside
            if (op.Text.Contains("IN"))
            {
                if (token.Type != TokenType.LParen)
                    throw new Exception("expected ( after in");
                var sb = new StringBuilder();
                var expect = new[] { "PeopleExtra", "FamilyExtra" }.Contains(c.ConditionName)
                    ? TokenType.String
                    : TokenType.Int;
                do
                {
                    token = lexer.Next(); // skip over ( or comma
                    if (token.Type != expect)
                        throw new Exception("expected string after IN (");
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(token.Text);
                    token = lexer.Next(); // skip to comma or )
                } while (token.Type == TokenType.Comma);
                if (token.Type != TokenType.RParen)
                    throw new Exception("expected closing ) after IN (");
                c.Comparison = op.Text.StartsWith("NOT")
                    ? CompareType.NotOneOf.ToString()
                    : CompareType.OneOf.ToString();
                SetRightSide(c, sb);
            }
            else
            {
                switch (op.Text)
                {
                    case "=":
                        if (token.Type == TokenType.String)
                        {
                            if (token.Text.StartsWith("*") && token.Text.EndsWith("*"))
                                c.Comparison = CompareType.Contains.ToString();
                            else if (token.Text.StartsWith("*"))
                                c.Comparison = CompareType.StartsWith.ToString();
                            else if (token.Text.EndsWith("*"))
                                c.Comparison = CompareType.EndsWith.ToString();
                        }
                        else
                            c.Comparison = CompareType.Equal.ToString();
                        break;
                    case "<>":
                        if (token.Type == TokenType.String)
                        {
                            if (token.Text.StartsWith("*") && token.Text.EndsWith("*"))
                                c.Comparison = CompareType.DoesNotContain.ToString();
                            else if (token.Text.StartsWith("*"))
                                c.Comparison = CompareType.DoesNotStartWith.ToString();
                            else if (token.Text.EndsWith("*"))
                                c.Comparison = CompareType.DoesNotEndWith.ToString();
                        }
                        else
                            c.Comparison = CompareType.NotEqual.ToString();
                        break;
                    case ">":
                        c.Comparison = CompareType.Greater.ToString();
                        break;
                    case "<":
                        c.Comparison = CompareType.Less.ToString();
                        break;
                    case ">=":
                        c.Comparison = CompareType.GreaterEqual.ToString();
                        break;
                    case "<=":
                        c.Comparison = CompareType.LessEqual.ToString();
                        break;
                }
                SetRightSide(c);
            }
            p?.AllConditions.Add(c.Id, c);
            return c;
        }

        private Condition AddConditions(Condition g)
        {
            while (token.Type != TokenType.RParen)
            {
                ParseCondition(g);
                if (token.Type == TokenType.RParen)
                {
                    if (!g.Comparison.HasValue())
                        g.Comparison = CompareType.AllTrue.ToString();
                    return g;
                }
                var andor = token.Type;
                token = lexer.Next(); // skip to next condition
                if (g.Comparison == CompareType.AllFalse.ToString() && andor != TokenType.And)
                    throw new Exception("Expected AND in AllFalse group");
                if (g.Comparison == CompareType.AllTrue.ToString() && andor != TokenType.And)
                    throw new Exception("Expected AND in AllTrue group");
                if (g.Comparison == CompareType.AnyTrue.ToString() && andor != TokenType.Or)
                    throw new Exception("Expected OR in AnyTrue group");
                if (g.Comparison.HasValue())
                    continue;
                switch (andor)
                {
                    case TokenType.And:
                        g.Comparison = CompareType.AllTrue.ToString();
                        break;
                    case TokenType.Or:
                        g.Comparison = CompareType.AnyTrue.ToString();
                        break;
                }
            }
            throw new Exception("missing ) in Group");
        }

        private void SetRightSide(Condition c, StringBuilder sb = null)
        {
            var text = sb?.ToString() ?? token.Text;
            switch (c.Compare2.ValueType())
            {
                case "text":
                    c.TextValue = text;
                    break;
                case "idtext":
                case "idvalue":
                    c.CodeIdValue = text;
                    break;
                case "date":
                    c.DateValue = text.ToDate();
                    break;
            }
            token = lexer.Next(); // skip past condition
        }

        private void ParseParam(Condition c)
        {
            if(token.Type == TokenType.RParen)
                return;
            var param = ParseParam(token.Text);
            SkipParamPastEqual();
            switch (param)
            {
                case Param.Program:
                    c.Program = token.Text;
                    break;
                case Param.Division:
                    c.Division = token.Text;
                    break;
                case Param.Organization:
                    c.Division = token.Text;
                    break;
                case Param.Schedule:
                    c.Schedule = token.Text;
                    break;
                case Param.OrgName:
                    c.OrgName = token.Text;
                    break;
                case Param.OrgStatus:
                    c.OrgStatus = token.Text;
                    break;
                case Param.StartDate:
                    c.StartDate = token.Text.ToDate();
                    break;
                case Param.EndDate:
                    c.EndDate = token.Text.ToDate();
                    break;
                case Param.Quarters:
                    c.Quarters = token.Text;
                    break;
                case Param.Age:
                    c.Age = token.Text.ToInt2();
                    break;
                case Param.Days:
                    c.Days = token.Text.ToInt();
                    break;
                case Param.Ministry:
                    c.Ministry = token.Text;
                    break;
                case Param.OnlineReg:
                    c.OnlineReg = token.Text;
                    break;
                case Param.OrgType2:
                    c.OrgType = token.Text;
                    break;
                case Param.Campus:
                    c.Campus = token.Text;
                    break;
                case Param.OrgType:
                    c.OrgType = token.Text;
                    break;
                case Param.PmmLabels:
                    c.Tags = token.Text;
                    break;
                case Param.Tags:
                    c.Tags = token.Text;
                    break;
                case Param.SavedQueryIdDesc:
                    c.SavedQuery = token.Text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            token = lexer.Next(); // skip to next comma or rparen
        }

        private void SkipParamPastEqual()
        {
            token = lexer.Next();
            if (token.Text != "=")
                throw new Exception("expected = in param");
            token = lexer.Next();
        }

        private Param ParseParam(string name)
        {
            switch (name)
            {
                case "Prog":
                    name = "Program";
                    break;
                case "Div":
                    name = "Division";
                    break;
                case "Org":
                    name = "Organization";
                    break;
                case "Sched":
                    name = "Schedule";
                    break;
                case "SavedQuery":
                    name = "SavedQueryIdDesc";
                    break;
            }
            return (Param)Enum.Parse(typeof(Param), name);
        }
    }
}