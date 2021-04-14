namespace Mallos.Searchable
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a simple filter class for making a simple check.
    /// </summary>
    /// <remarks>
    /// This class is designed to make it easier for people to get
    /// started using the library.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleFilter<T> : IFilter<T>
        where T : class
    {
        /// <inheritdoc />
        public abstract string Key { get; }

        /// <summary>
        /// Returns whether the object matches the filter.
        /// </summary>
        /// <param name="item">The item we are checking.</param>
        /// <param name="value">
        /// The value from the filter.
        ///
        /// If this is a "is" filter then the <paramref name="value"/>
        /// will be null as the value is the key.
        ///
        /// For example: "filter:value" here the value will be "value".
        /// </param>
        /// <returns>Whether the object matches the filter.</returns>
        protected abstract bool Check(T item, string value);

        /// <inheritdoc />
        IEnumerable<T> IFilter<T>.Negative(IEnumerable<T> values, string value)
            => values.Where(x => !Check(x, value));

        /// <inheritdoc />
        IEnumerable<T> IFilter<T>.Positive(IEnumerable<T> values, string value)
            => values.Where(x => Check(x, value));
    }
}
