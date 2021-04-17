namespace Mallos.Searchable.Attributes
{
    using System;

    public enum FilterMatchType
    {
        Match,
        Contains,
        StartWith,
        EndWith
    }

    public sealed class FilterStringMatchAttribute : FilterBaseAttribute
    {
        public FilterMatchType FilterMatchType { get; }

        public StringComparison ComparisonType { get; }

        public FilterStringMatchAttribute()
            : this(FilterMatchType.Contains)
        {
        }

        public FilterStringMatchAttribute(FilterMatchType FilterMatchType)
            : this(FilterMatchType, StringComparison.OrdinalIgnoreCase)
        {
        }

        public FilterStringMatchAttribute(FilterMatchType FilterMatchType, StringComparison comparisonType)
        {
            this.FilterMatchType = FilterMatchType;
            this.ComparisonType = comparisonType;
        }

        public static bool Execute(
            string value,
            string query,
            FilterMatchType FilterMatchType = FilterMatchType.Contains,
            StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            switch (FilterMatchType)
            {
                case FilterMatchType.Match:
                    return value.Equals(query, comparisonType);

                case FilterMatchType.Contains:
#if NETSTANDARD2_1
                    return value.Contains(query, comparisonType);
#else
                    // NOTE: I am not sure this is correct
                    switch (comparisonType)
                    {
                        default:
                        case StringComparison.InvariantCulture:
                        case StringComparison.CurrentCulture:
                        case StringComparison.Ordinal:
                            return value.Contains(query);

                        case StringComparison.CurrentCultureIgnoreCase:
                        case StringComparison.OrdinalIgnoreCase:
                            return value.ToLower().Contains(query.ToLower());

                        case StringComparison.InvariantCultureIgnoreCase:
                            return value.ToLowerInvariant().Contains(query.ToLowerInvariant());
                    }
#endif

                case FilterMatchType.StartWith:
                    return value.StartsWith(query, comparisonType);

                case FilterMatchType.EndWith:
                    return value.EndsWith(query, comparisonType);

                default:
                    throw new NotSupportedException(nameof(comparisonType));
            }
        }
    }
}
