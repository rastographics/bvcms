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
        
        private Token Token => lexer.Token;

        private void NextToken(params TokenType[] args)
        {
            if (lexer.Next() == false)
                Token.Type = TokenType.RParen;
            if(!args.Contains(Token.Type))
                throw new Exception($@"Expected {string.Join(",", args.Select(aa => aa.ToString()))}
{lexer.Line.Insert(lexer.Position, "^")}
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
        private Condition ParseCondition(Condition p = null)
        {
            // if no surrounding group, create one
            //if (p == null && Token.Type != TokenType.LParen)
            //{
            //}
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
                    NextToken(TokenType.Name, TokenType.Func, TokenType.LParen);
                    c.ConditionName = "Group";
                    return AddConditions(c);
                case TokenType.Name:
                    c.ConditionName = Token.Text;
                    if (c.ConditionName == "MatchAnything")
                    {
                        NextToken(TokenType.And, TokenType.Or,TokenType.AndNot, TokenType.RParen);
                        return c;
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

                c.Comparison = op.Text.StartsWith("NOT")
                    ? CompareType.NotOneOf.ToString()
                    : CompareType.OneOf.ToString();
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
                                c.Comparison = CompareType.Contains.ToString();
                            else if (Token.Text.StartsWith("*"))
                                c.Comparison = CompareType.StartsWith.ToString();
                            else if (Token.Text.EndsWith("*"))
                                c.Comparison = CompareType.EndsWith.ToString();
                        }
                        else
                            c.Comparison = CompareType.Equal.ToString();
                        break;
                    case "<>":
                        if (Token.Type == TokenType.String)
                        {
                            if (Token.Text.StartsWith("*") && Token.Text.EndsWith("*"))
                                c.Comparison = CompareType.DoesNotContain.ToString();
                            else if (Token.Text.StartsWith("*"))
                                c.Comparison = CompareType.DoesNotStartWith.ToString();
                            else if (Token.Text.EndsWith("*"))
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
            return c;
        }

        private Condition AddConditions(Condition g)
        {
            while (Token.Type != TokenType.RParen)
            {
                ParseCondition(g);
                if (Token.Type == TokenType.RParen)
                {
                    if (!g.Comparison.HasValue())
                        g.Comparison = CompareType.AllTrue.ToString();
                    return g;
                }
                var andOrNot = Token.Type;
                NextToken(TokenType.Name, TokenType.Func, TokenType.LParen);
                if (g.Comparison == CompareType.AllFalse.ToString() && andOrNot != TokenType.AndNot)
                    throw new Exception("Expected AND NOT in AllFalse group");
                if (g.Comparison == CompareType.AllTrue.ToString() && andOrNot != TokenType.And)
                    throw new Exception("Expected AND in AllTrue group");
                if (g.Comparison == CompareType.AnyTrue.ToString() && andOrNot != TokenType.Or)
                    throw new Exception("Expected OR in AnyTrue group");
                if (g.Comparison.HasValue())
                    continue;
                switch (andOrNot)
                {
                    case TokenType.And:
                        g.Comparison = CompareType.AllTrue.ToString();
                        break;
                    case TokenType.Or:
                        g.Comparison = CompareType.AnyTrue.ToString();
                        break;
                    case TokenType.AndNot:
                        g.Comparison = CompareType.AllFalse.ToString();
                        break;
                }
            }
            throw new Exception("missing ) in Group");
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
            NextToken(TokenType.And, TokenType.Or, TokenType.RParen);
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