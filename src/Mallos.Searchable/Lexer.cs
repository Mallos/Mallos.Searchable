namespace Mallos.Searchable
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    enum TokenType
    {
        EMPTY,
        EOF,
        Eoln,
        String,
        Separator,
        Negative,
        EQ
    }

    struct Token
    {
        public static Token Empty => new Token(TokenType.EMPTY, string.Empty, 0, 0);

        public readonly TokenType Type;
        public readonly string Value;

        public readonly int Row;
        public readonly int Col;

        public Token(TokenType type, string value, int row, int col)
        {
            this.Type = type;
            this.Value = value;
            this.Row = row;
            this.Col = col;
        }

        public ExpressionLocation GetStartLocation()
            => new ExpressionLocation(this.Row, this.Col);

        public ExpressionLocation GetEndLocation()
            => new ExpressionLocation(this.Row, this.Col + this.Value.Length);

        public ExpressionLocationRange GetLocation()
            => new ExpressionLocationRange(this.GetStartLocation(), this.GetEndLocation());

        public override string ToString() => $"('{Type}', '{Value}')";
    }

    class Lexer
    {
        private static readonly Dictionary<char, TokenType> singleCharTokens = new Dictionary<char, TokenType>()
        {
            { ':', TokenType.EQ },
            { ' ', TokenType.Separator },
            { '-', TokenType.Negative },
            { '\n', TokenType.Eoln },
            { '\0', TokenType.EOF },
        };

        public Token CurrentToken { get; private set; }

        public CultureInfo Culture { get; set; }

        private readonly TextReader reader;
        private readonly StringBuilder sb = new StringBuilder();
        private Token? peekToken;
        private char currentChar;
        private int col = -1;
        private int row = 1;

        public Lexer(string content, CultureInfo culture = null)
            : this(new StringReader(content), culture)
        {
        }

        public Lexer(TextReader reader, CultureInfo culture = null)
        {
            this.reader = reader;
            this.Culture = culture ?? CultureInfo.InvariantCulture;

            NextChar();
            NextToken();
        }

        public Token PeekToken()
        {
            if (this.peekToken.HasValue)
            {
                return this.peekToken.Value;
            }
            else
            {
                return (this.peekToken = ReadToken()).Value;
            }
        }

        public Token NextToken(int nth)
        {
            for (int i = 0; i < nth; i++)
            {
                NextToken();
            }
            return this.CurrentToken;
        }

        public Token NextToken()
        {
            if (this.peekToken.HasValue)
            {
                this.CurrentToken = this.peekToken.Value;
                this.peekToken = null;
            }
            else
            {
                this.CurrentToken = ReadToken();
            }
            return this.CurrentToken;
        }

        public bool Next()
        {
            return this.NextToken().Type != TokenType.EOF;
        }

        private Token ReadToken()
        {
            if (currentChar == '\0' || currentChar == '\r' || currentChar == '\n')
            {
                // TODO: this breaks multiline
                return new Token(TokenType.EOF, "", row, col);
            }

            if (singleCharTokens.ContainsKey(currentChar))
            {
                var lastCharacter = currentChar;
                var result = singleCharTokens[lastCharacter];

                NextChar();

                return new Token(result, lastCharacter.ToString(), row, col);
            }
            else if (currentChar == '\"' || currentChar == '\'')
            {
                NextChar();

                sb.Clear();
                while ((currentChar != '\"' || currentChar != '\'') && !(currentChar == '\0' || currentChar == '\r' || currentChar == '\n'))
                {
                    sb.Append(currentChar);
                    NextChar();
                }
                NextChar();

                var result = sb.ToString();

                int unicodeIndex;
                while ((unicodeIndex = result.IndexOf("\\u")) != -1)
                {
                    var unicodeNumber = result.Substring(unicodeIndex + 2, 4);
                    if (int.TryParse(
                            unicodeNumber,
                            NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture,
                            out var unicodeValue
                        ))
                    {
                        var unicodeText = char.ConvertFromUtf32(unicodeValue).ToString();
                        var tmp = result.Substring(unicodeIndex, 6);
                        result = result.Replace(tmp, unicodeText);
                    }
                }

                return new Token(TokenType.String, result[0..^1], row, col);
            }
            else
            {
                sb.Clear();

                char lastChar = 'a';
                while (!singleCharTokens.ContainsKey(currentChar))
                {
                    sb.Append(currentChar);

                    lastChar = currentChar;
                    NextChar();
                }

                var sbResult = sb.ToString();
                if (sbResult.Length == 0)
                {
                    throw new System.ArgumentException($"Unexpect character: {currentChar}");
                }

                return new Token(TokenType.String, sbResult, row, col);
            }
        }

        private void NextChar()
        {
            int ch = this.reader.Read();
            col++;

            currentChar = ch < 0 ? '\0' : (char)ch;
        }
    }
}
