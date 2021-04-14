namespace Mallos.Searchable
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the definitions for a positive and negative filter.
    /// </summary>
    /// <remarks>
    /// To make specialized queries having both positive and negative
    /// can make it easier to optimize the requests.
    /// </remarks>
    /// <typeparam name="T">The filterable object.</typeparam>
    public interface IFilter<T>
        where T : class
    {
        /// <summary>
        /// Returns the key which is used to identify the filter.
        /// </summary>
        /// <remarks>
        /// If this is a "is" filter this will act as the value.
        /// For example: "is:value" here the value is the <see cref="Key"/>.
        /// </remarks>
        string Key { get; }

        /// <summary>
        /// Applies a positive filter on the <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <param name="values">The filterable collection, this should be returned.</param>
        /// <param name="value">
        /// The value from the filter.
        ///
        /// If this is a "is" filter then the <paramref name="value"/>
        /// will be null as the value is the key.
        ///
        /// For example: "filter:value" here the value will be "value".
        /// </param>
        /// <returns>The updated filterable collection.</returns>
        IEnumerable<T> Positive(IEnumerable<T> values, string value);

        /// <summary>
        /// Applies a negative filter on the <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <param name="values">The filterable collection, this should be returned.</param>
        /// <param name="value">
        /// The value from the filter.
        ///
        /// If this is a "is" filter then the <paramref name="value"/>
        /// will be null as the value is the key.
        ///
        /// For example: "filter:value" here the value will be "value".
        /// </param>
        /// <returns>The updated filterable collection.</returns>
        IEnumerable<T> Negative(IEnumerable<T> values, string value);
    }
}
