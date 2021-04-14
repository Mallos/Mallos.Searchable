namespace Mallos.Searchable
{
    public struct Expression
    {
        public bool IsNegative { get; }

        public string Key { get; }

        public string Value { get; }

        public ExpressionLocationRange Location { get; }

        public ExpressionLocationRange KeyLocation { get; }

        public ExpressionLocationRange ValueLocation { get; }

        public Expression(
            bool isNegative,
            string key,
            string value,
            ExpressionLocationRange location,
            ExpressionLocationRange keyLocation,
            ExpressionLocationRange valueLocation)
        {
            this.IsNegative = isNegative;
            this.Key = key;
            this.Value = value;
            this.Location = location;
            this.KeyLocation = keyLocation;
            this.ValueLocation = valueLocation;
        }

        public override string ToString()
        {
            return $"<Negative: {this.IsNegative}, Key: {this.Key}, Value: {this.Value}>";
        }
    }
}
