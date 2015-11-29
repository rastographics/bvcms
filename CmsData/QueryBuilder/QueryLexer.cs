using System;
using System.IO;
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
        Op,
        Func,
        Name,
        LParen,
        RParen,
        Space,
        Comma,
        In,
        Not
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
        private int lineNumber;
        private int position;
        private string lineRemaining;

        public QueryLexer(string text)
        {
            tokenDefs = new[]
            {
                new TokenDef(TokenType.LParen,  @"\("),
                new TokenDef(TokenType.RParen,  @"\)"),
                new TokenDef(TokenType.String,  @"(')(?:\\\1|.)*?\1"),
                new TokenDef(TokenType.Num,     @"[-+]?\d*\.\d+([eE][-+]?\d+)?"),
                new TokenDef(TokenType.Int,     @"[-+]?\d+(\[[^]]*?\])?"),
                new TokenDef(TokenType.And,     @"AND"),
                new TokenDef(TokenType.Or,      @"OR"),
                new TokenDef(TokenType.Not,     @"NOT"),
                new TokenDef(TokenType.Op,      @"(>=|<=|=|<|>|IN(?=\s*\()|NOT\sIN(?=\s*\())"),
                new TokenDef(TokenType.Func,    @"[*<>\?\-+/A-Za-z->!]+(?=\()"),
                new TokenDef(TokenType.Name,    @"[*<>\?\-+/A-Za-z->!]+"),
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
                lineRemaining = reader.ReadLine();
                ++lineNumber;
                position = 0;
            } while (lineRemaining != null && lineRemaining.Length == 0);
        }

        public Token Next()
        {
            var token = GetNext();
            return token.Type == TokenType.Space ? Next() : token;
        }

        private Token GetNext()
        {
            if (lineRemaining == null)
                return null;
            foreach (var def in tokenDefs)
            {
                var matched = def.Matcher.Match(lineRemaining);
                if (matched <= 0)
                    continue;
                position += matched;
                var token = new Token
                {
                    Type = def.TokenType,
                    Text = lineRemaining.Substring(0, matched)
                };
                lineRemaining = lineRemaining.Substring(matched);
                if (lineRemaining.Length == 0)
                    NextLine();
                return token;
            }
            throw new Exception($"Unable to match against any tokens at line {lineNumber} position {position} \"{lineRemaining}\"");
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
