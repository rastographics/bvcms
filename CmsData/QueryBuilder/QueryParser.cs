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
        private QueryParser(string s) { lexer = new QueryLexer(s); }
        private string PositionLine => $"{lexer.Line.Insert(lexer.Position, "^")}";
        
        private Token Token => lexer.Token;

        private void NextToken(params TokenType[] args)
        {
            if (lexer.Next() == false)
                Token.Type = TokenType.RParen;
            if(!args.Contains(Token.Type))
                throw new Exception($@"Expected {string.Join(",", args.Select(aa => aa.ToString()))}
{PositionLine}
");
        }
        private void NextToken(string text)
        {
            lexer.Next();
            if(Token.Text != text)
                throw new Exception($"Expected {text}");
        }

        public static Condition Parse(string s)
        {
            var m = new QueryParser(s);
            var p = Condition.CreateAllGroup();
            var c = m.AddConditions(p);
            if (p.Conditions.Count() == 1 && c.IsGroup)
                return c;
            return p;
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
                    AddConditions(c);
                    return;
                case TokenType.Name:
                    c.ConditionName = Token.Text;
                    if (c.ConditionName == "MatchAnything")
                    {
                        NextToken(TokenType.And, TokenType.Or,TokenType.AndNot, TokenType.RParen);
                        return;
                    }
                    break;
                case TokenType.Func:
                    c.ConditionName = Token.Text;
                    NextToken(TokenType.LParen);
                    do
                        ParseParam(c);
                    while (Token.Type == TokenType.Comma);
                    if (Token.Type != TokenType.RParen)
                        throw new Exception("missing ) on function parameters");
                    break;
            }
            NextToken(TokenType.Op); // get operator
            var op = Token;
            if (op.Text.Contains("IN"))
            {
                NextToken(TokenType.LParen);
                var sb = new StringBuilder();
                var expect = new[] { "PeopleExtra", "FamilyExtra" }.Contains(c.ConditionName)
                    ? TokenType.String
                    : TokenType.Int;
                do
                {
                    NextToken(expect);
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

        private Condition AddConditions(Condition g)
        {
            NextToken(TokenType.Name, TokenType.Func, TokenType.Not, TokenType.LParen);
            if(Token.Type == TokenType.Not)
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
            throw new Exception("missing ) in Group");
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
            if (g.ComparisonType == CompareType.AllFalse && Token.Type != TokenType.AndNot)
                throw new Exception("Expected AND NOT in AllFalse group");
            if (g.ComparisonType == CompareType.AllTrue && Token.Type != TokenType.And)
                throw new Exception("Expected AND in AllTrue group");
            if (g.ComparisonType == CompareType.AnyTrue && Token.Type != TokenType.Or)
                throw new Exception("Expected OR in AnyTrue group");
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
                    c.CodeIdValue = text;
                    break;
                case "date":
                    c.DateValue = text.ToDate();
                    break;
            }
            NextToken(TokenType.And, TokenType.Or, TokenType.AndNot, TokenType.RParen);
        }

        private void ParseParam(Condition c)
        {
            NextToken(TokenType.Name,TokenType.RParen);
            if (Token.Type == TokenType.RParen)
                return;
            var param = ParseParam(Token.Text);
            NextToken("=");
            NextToken(TokenType.String, TokenType.Int);
            switch (param)
            {
                case Param.Program:
                    c.Program = Token.Text;
                    break;
                case Param.Division:
                    c.Division = Token.Text;
                    break;
                case Param.Organization:
                    c.Division = Token.Text;
                    break;
                case Param.Schedule:
                    c.Schedule = Token.Text;
                    break;
                case Param.OrgName:
                    c.OrgName = Token.Text;
                    break;
                case Param.OrgStatus:
                    c.OrgStatus = Token.Text;
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
                    c.Ministry = Token.Text;
                    break;
                case Param.OnlineReg:
                    c.OnlineReg = Token.Text;
                    break;
                case Param.OrgType2:
                    c.OrgType = Token.Text;
                    break;
                case Param.Campus:
                    c.Campus = Token.Text;
                    break;
                case Param.OrgType:
                    c.OrgType = Token.Text;
                    break;
                case Param.PmmLabels:
                    c.Tags = Token.Text;
                    break;
                case Param.Tags:
                    c.Tags = Token.Text;
                    break;
                case Param.SavedQueryIdDesc:
                    c.SavedQuery = Token.Text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            NextToken(TokenType.Comma, TokenType.RParen);
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
            return (Param) Enum.Parse(typeof (Param), name);
        }
    }
}