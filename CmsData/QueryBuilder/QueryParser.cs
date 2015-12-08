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
        private readonly string text;
        public QueryParser(string s)
        {
            text = s;
            lexer = new QueryLexer(s);
        }
        private string PositionLine => $"{lexer.Line.Insert(lexer.Position, "^")}";

        private Token Token => lexer.Token;

        private void NextToken(params TokenType[] args)
        {
            if (lexer.Next() == false)
                Token.Type = TokenType.RParen;
            if (args.Contains(Token.Type))
                return;
            if (Token.Type == TokenType.Name && args.Contains(TokenType.Int) && Token.Text.Equal("true"))
            {
                Token.Text = "1[True]";
                Token.Type = TokenType.Int;
                return;
            }
            throw new QueryParserException($@"Expected {string.Join(",", args.Select(aa => aa.ToString()))}
{PositionLine}
");
        }
        private void NextToken(string text)
        {
            lexer.Next();
            if (Token.Text != text)
                throw new QueryParserException($"Expected {text}");
        }

        private void ParseCondition(Condition p = null)
        {
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
            p?.AllConditions.Add(c.Id, c);
            switch (Token.Type)
            {
                case TokenType.LParen:
                    c.ConditionName = "Group";
                    ParseConditions(c);
                    return;
                case TokenType.Name:
                    c.ConditionName = Token.Text;
                    break;
                case TokenType.Func:
                    c.ConditionName = Token.Text;
                    NextToken(TokenType.LParen);
                    do
                        ParseParam(c);
                    while (Token.Type == TokenType.Comma);
                    if (Token.Type != TokenType.RParen)
                        throw new QueryParserException("missing ) on function parameters");
                    break;
            }
            NextToken(TokenType.Op); // get operator
            var op = Token;
            if (op.Text.Contains("IN"))
            {
                NextToken(TokenType.LParen);
                var sb = new StringBuilder();
                var expect = c.FieldInfo.Type == FieldType.CodeStr
                    ? TokenType.String
                    : TokenType.Int;
                do
                {
                    NextToken(expect, TokenType.RParen);
                    if (Token.Type == TokenType.RParen)
                        continue;
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(Token.Text);
                    NextToken(TokenType.Comma, TokenType.RParen);
                }
                while (Token.Type == TokenType.Comma);

                c.SetComparisonType(op.Text.StartsWith("NOT")
                    ? CompareType.NotOneOf
                    : CompareType.OneOf);
                SetRightSide(c, sb);
            }
            else
            {
                NextToken(TokenType.String, TokenType.Int, TokenType.Num);
                switch (op.Text)
                {
                    case "=":
                        if (Token.Type == TokenType.String)
                        {
                            if (Token.Text.StartsWith("*") && Token.Text.EndsWith("*"))
                                c.SetComparisonType(CompareType.Contains);
                            else if (Token.Text.StartsWith("*"))
                                c.SetComparisonType(CompareType.StartsWith);
                            else if (Token.Text.EndsWith("*"))
                                c.SetComparisonType(CompareType.EndsWith);
                        }
                        else
                            c.SetComparisonType(CompareType.Equal);
                        break;
                    case "<>":
                        if (Token.Type == TokenType.String)
                        {
                            if (Token.Text.StartsWith("*") && Token.Text.EndsWith("*"))
                                c.SetComparisonType(CompareType.DoesNotContain);
                            else if (Token.Text.StartsWith("*"))
                                c.SetComparisonType(CompareType.DoesNotStartWith);
                            else if (Token.Text.EndsWith("*"))
                                c.SetComparisonType(CompareType.DoesNotEndWith);
                        }
                        else
                            c.SetComparisonType(CompareType.NotEqual);
                        break;
                    case ">":
                        c.SetComparisonType(CompareType.Greater);
                        break;
                    case "<":
                        c.SetComparisonType(CompareType.Less);
                        break;
                    case ">=":
                        c.SetComparisonType(CompareType.GreaterEqual);
                        break;
                    case "<=":
                        c.SetComparisonType(CompareType.LessEqual);
                        break;
                }
                SetRightSide(c);
            }
        }

        public Condition ParseConditions(Condition g)
        {
            NextToken(TokenType.Name, TokenType.Func, TokenType.Not, TokenType.LParen);
            if (Token.Type == TokenType.Not)
            {
                g.SetComparisonType(CompareType.AllFalse);
                NextToken(TokenType.Name, TokenType.Func, TokenType.LParen);
            }
            while (Token.Type != TokenType.RParen)
            {
                ParseCondition(g);
                if (Token.Type == TokenType.RParen)
                {
                    if (!g.Comparison.HasValue())
                        g.SetComparisonType(CompareType.AllTrue);
                    NextToken(TokenType.And, TokenType.Or, TokenType.RParen);
                    return g;
                }
                SetComparisionType(g);
                NextToken(TokenType.Name, TokenType.Func, TokenType.LParen);
            }
            throw new QueryParserException("missing ) in Group");
        }

        private void SetComparisionType(Condition g)
        {
            CheckAndOrNotConsistency(g);
            if (!g.Comparison.HasValue())
                switch (Token.Type)
                {
                    case TokenType.And:
                        g.SetComparisonType(CompareType.AllTrue);
                        break;
                    case TokenType.Or:
                        g.SetComparisonType(CompareType.AnyTrue);
                        break;
                    case TokenType.AndNot:
                        g.SetComparisonType(CompareType.AllFalse);
                        break;
                }
        }

        private void CheckAndOrNotConsistency(Condition g)
        {
            if (!g.Comparison.HasValue())
                return;
            if (g.ComparisonType == CompareType.AllFalse && Token.Type != TokenType.AndNot)
                throw new QueryParserException("Expected AND NOT in AllFalse group");
            if (g.ComparisonType == CompareType.AllTrue && Token.Type != TokenType.And)
                throw new QueryParserException("Expected AND in AllTrue group");
            if (g.ComparisonType == CompareType.AnyTrue && Token.Type != TokenType.Or)
                throw new QueryParserException("Expected OR in AnyTrue group");
        }

        private void SetRightSide(Condition c, StringBuilder sb = null)
        {
            var text = sb?.ToString() ?? Token.Text;
            switch (c.Compare2.ValueType())
            {
                case "text":
                    c.TextValue = text;
                    break;
                case "idtext":
                case "idvalue":
                    c.CodeIdValue = Token2Csv();
                    break;
                case "date":
                    c.DateValue = text.ToDate();
                    break;
            }
            NextToken(TokenType.And, TokenType.Or, TokenType.AndNot, TokenType.RParen);
        }

        private void ParseParam(Condition c)
        {
            NextToken(TokenType.Name, TokenType.RParen);
            if (Token.Type == TokenType.RParen)
                return;
            var param = ParamEnum(Token.Text);
            NextToken("=");
            NextToken(TokenType.String, TokenType.Int);
            switch (param)
            {
                case Param.Program:
                    c.Program = Token2Csv();
                    break;
                case Param.Division:
                    c.Division = Token2Csv();
                    break;
                case Param.Organization:
                    c.Division = Token2Csv();
                    break;
                case Param.Schedule:
                    c.Schedule = Token2Csv();
                    break;
                case Param.OrgName:
                    c.OrgName = Token2Csv();
                    break;
                case Param.OrgStatus:
                    c.OrgStatus = Token2Csv();
                    break;
                case Param.StartDate:
                    c.StartDate = Token.Text.ToDate();
                    break;
                case Param.EndDate:
                    c.EndDate = Token.Text.ToDate();
                    break;
                case Param.Quarters:
                    c.Quarters = Token.Text;
                    break;
                case Param.Age:
                    c.Age = Token.Text.ToInt2();
                    break;
                case Param.Days:
                    c.Days = Token.Text.ToInt();
                    break;
                case Param.Ministry:
                    c.Ministry = Token2Csv();
                    break;
                case Param.OnlineReg:
                    c.OnlineReg = Token2Csv();
                    break;
                case Param.OrgType2:
                    c.OrgType = Token2Csv();
                    break;
                case Param.Campus:
                    c.Campus = Token2Csv();
                    break;
                case Param.OrgType:
                    c.OrgType = Token2Csv();
                    break;
                case Param.PmmLabels:
                case Param.Tag:
                    AddTagParam(c);
                    break;
                case Param.SavedQueryIdDesc:
                    c.SavedQuery = Token.Text;
                    break;
                default:
                    throw new QueryParserException($"No Such Param:{param}");
            }
            NextToken(TokenType.Comma, TokenType.RParen);
        }

        private void AddTagParam(Condition c)
        {
            var tag = Token2Csv();
            c.Tags = c.Tags.HasValue() ? $"{c};{tag}" : tag;
        }

        private string Token2Csv()
        {
            if (Token.Type != TokenType.Int)
                return Token.Text;
            var a = Token.Text.SplitStr("[]");
            return a.Length > 1 ? $"{a[0]},{a[1]}" : a[0];
        }

        private Param ParamEnum(string name)
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
                case "VisitNumber":
                case "Field":
                case "Name":
                case "FundIdOrBlank":
                case "FundIdOrNullForAll":
                case "AgeRange":
                case "NthVisitNumber":
                case "TopNumber":
                case "UsernameOrPeopleId":
                case "MeetingId":
                case "NumberOfDaysForNoAttendance":
                case "AttendCreditId":
                    name = "Quarters";
                    break;
            }
            return (Param)Enum.Parse(typeof(Param), name);
        }

        public class QueryParserException : Exception
        {
            public QueryParserException(string message) : base(message)
            {
            }
        }
    }
}