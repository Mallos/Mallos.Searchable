namespace Mallos.Searchable
{
    public struct ExpressionLocation
    {
        public int Row { get; }

        public int Col { get; }

        public ExpressionLocation(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public override string ToString()
        {
            return $"<Ln: {this.Row}, Ch: {this.Col}>";
        }
    }

    public struct ExpressionLocationRange
    {
        public ExpressionLocation Start { get; }

        public ExpressionLocation End { get; }

        public ExpressionLocationRange(
            ExpressionLocation start,
            ExpressionLocation end)
        {
            this.Start = start;
            this.End = end;
        }

        public override string ToString()
        {
            return $"<{this.Start} -> {this.End}>";
        }
    }
}
