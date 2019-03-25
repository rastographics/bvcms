using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CmsData.QueryBuilder
{
    public enum TokenType
    {
        String,
        Num,
        Int,
        And,
        Or,
        AndNot,
        OrNot,
        Op,
        Func,
        Name,
        LParen,
        RParen,
        Space,
        Comma,
        In,
        Not,
        Comment
    }
    public class Token
    {
        public TokenType Type { get; set; }
        public string Text { get; set; }
    }

    public class QueryLexer : IDisposable
    {
        public Token Token;

        private readonly TextReader reader;
        private readonly TokenDef[] tokenDefs;
        public int LineNumber;
        public int Position;
        public string Line;
        public string LineRemaining;

        public QueryLexer(string text)
        {
            tokenDefs = new[]
            {
                new TokenDef(TokenType.Comment,  @"/\*.*\*/"),
                new TokenDef(TokenType.LParen,  @"\("),
                new TokenDef(TokenType.RParen,  @"\)"),
                new TokenDef(TokenType.String,  @"'((?>[^']+|'')*)'"),
                new TokenDef(TokenType.Num,     @"[-+]?\d*\.\d+([eE][-+]?\d+)?"),
                new TokenDef(TokenType.Int,     @"[-+]?\d+(\[[^]]*?\])?"),
                new TokenDef(TokenType.Op,      @"(>=|<=|=|<>|<|>|IN(?=\s*\()|NOT\sIN(?=\s*\())"),
                new TokenDef(TokenType.AndNot,  @"AND\s*NOT"),
                new TokenDef(TokenType.And,     @"AND"),
                new TokenDef(TokenType.Or,      @"OR"),
                new TokenDef(TokenType.Not,     @"NOT"),
                new TokenDef(TokenType.Func,    @"[A-Za-z][A-Za-z0-9]+(?=\()"),
                new TokenDef(TokenType.Name,    @"[A-Za-z][A-Za-z0-9]+"),
                new TokenDef(TokenType.Space,   @"\s*"),
                new TokenDef(TokenType.Comma,   @","),
            };
            reader = new StringReader(text);
            NextLine();
        }

        private void NextLine()
        {
            do
            {
                Line = reader.ReadLine();
                LineRemaining = Line;
                ++LineNumber;
                Position = 0;
            } while (LineRemaining != null && LineRemaining.Length == 0);
        }

        private readonly TokenType[] skipTypes = { TokenType.Space, TokenType.Comment };
        public bool Next()
        {
            while (true)
            {
                var ret = GetNext();
                if (ret && skipTypes.Contains(Token.Type))
                    continue;
                if (Token.Type == TokenType.String)
                    Token.Text = Token.Text.Trim('\'');
                return ret;
            }
        }

        private bool GetNext()
        {
            if (LineRemaining == null)
                return false;
            foreach (var def in tokenDefs)
            {
                var matched = def.Matcher.Match(LineRemaining);
                if (matched <= 0)
                    continue;
                Position += matched;
                Token = new Token
                {
                    Type = def.TokenType,
                    Text = LineRemaining.Substring(0, matched)
                };
                LineRemaining = LineRemaining.Substring(matched);
                if (LineRemaining.Length == 0)
                    NextLine();
                return true;
            }
            throw new QueryParser.QueryParserException($"Unable to match against any tokens at line {LineNumber} position {Position} \"{LineRemaining}\"");
        }

        private class TokenDef
        {
            public readonly RegexMatcher Matcher;
            public readonly TokenType TokenType;

            public TokenDef(TokenType type, string regex)
            {
                Matcher = new RegexMatcher(regex);
                TokenType = type;
            }
        }

        private class RegexMatcher
        {
            private readonly Regex regex;
            public RegexMatcher(string regex)
            {
                this.regex = new Regex($"^{regex}");
            }

            public int Match(string text)
            {
                var m = regex.Match(text);
                return m.Success ? m.Length : 0;
            }

            public override string ToString()
            {
                return regex.ToString();
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
