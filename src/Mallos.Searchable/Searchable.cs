namespace Mallos.Searchable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Searchable<T>
        where T : class
    {
        /// <summary>
        /// Gets the is filters.
        /// </summary>
        public SearchableCollection<T> IsFilters { get; } = new SearchableCollection<T>();

        /// <summary>
        /// Gets the queries.
        /// </summary>
        public SearchableCollection<T> Filters { get; } = new SearchableCollection<T>();

        /// <summary>
        /// Returns a filtered collection based on the free text.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="negative"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> FreeTextFilter(IEnumerable<T> values, bool negative, string text)
        {
            return values;
        }

        /// <summary>
        /// Returns a filtered collection based on the query string.
        /// </summary>
        /// <param name="values">The collection to filter.</param>
        /// <param name="query">The query string.</param>
        /// <returns>A filtered collection.</returns>
        public IEnumerable<T> Search(IEnumerable<T> values, string query)
        {
            if (values.Count() == 0)
                return values;

            var expressions = this.Analyze(query);
            foreach (var expression in expressions)
            {
                if (expression.Key == null)
                {
                    values = FreeTextFilter(values, expression.IsNegative, expression.Value);
                }
                else if (expression.Key.Equals("is", StringComparison.OrdinalIgnoreCase))
                {
                    var q = this.IsFilters.FastFind(expression.Value);
                    if (q != null)
                    {
                        values = expression.IsNegative
                            ? q.Negative(values, expression.Key)
                            : q.Positive(values, expression.Key);
                    }
                }
                else
                {
                    var q = this.Filters.FastFind(expression.Key);
                    if (q != null)
                    {
                        values = expression.IsNegative
                            ? q.Negative(values, expression.Value)
                            : q.Positive(values, expression.Value);
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Returns a analyzed result of the filter.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <returns>A list of expressions.</returns>
        public List<Expression> Analyze(string query)
        {
            var list = new List<Expression>();
            var lexer = new Lexer(query);

            while (lexer.CurrentToken.Type != TokenType.EOF)
            {
                var startLocation = new ExpressionLocation(lexer.CurrentToken.Row, lexer.CurrentToken.Col);

                var negative = lexer.CurrentToken.Type == TokenType.Negative;
                if (negative)
                    lexer.Next();

                if (lexer.CurrentToken.Type == TokenType.String)
                {
                    if (lexer.PeekToken().Type == TokenType.EQ)
                    {
                        var keyToken = lexer.CurrentToken;
                        lexer.NextToken(2);
                        var valueToken = lexer.CurrentToken;

                        var endLocation = new ExpressionLocation(
                            lexer.CurrentToken.Row,
                            lexer.CurrentToken.Col + lexer.CurrentToken.Value.Length
                        );

                        var keyLocationRange = new ExpressionLocationRange(
                            new ExpressionLocation(
                                keyToken.Row,
                                keyToken.Col
                            ),
                            new ExpressionLocation(
                                keyToken.Row,
                                keyToken.Col + keyToken.Value.Length
                            )
                        );

                        var valueLocationRange = new ExpressionLocationRange(
                            new ExpressionLocation(
                                valueToken.Row,
                                valueToken.Col
                            ),
                            new ExpressionLocation(
                                valueToken.Row,
                                valueToken.Col + valueToken.Value.Length
                            )
                        );

                        var locationRange = new ExpressionLocationRange(startLocation, endLocation);

                        var expression = new Expression(
                            negative,
                            keyToken.Value,
                            valueToken.Value,
                            locationRange,
                            keyLocationRange,
                            valueLocationRange
                        );

                        list.Add(expression);
                    }
                    else
                    {
                        var endLocation = new ExpressionLocation(
                            lexer.CurrentToken.Row,
                            lexer.CurrentToken.Col + lexer.CurrentToken.Value.Length
                        );

                        var locationRange = new ExpressionLocationRange(startLocation, endLocation);

                        var expression = new Expression(
                            negative,
                            null,
                            lexer.CurrentToken.Value,
                            locationRange,
                            locationRange,
                            locationRange
                        );

                        list.Add(expression);
                    }
                }

                lexer.Next();
            }

            return list;
        }
    }
}
