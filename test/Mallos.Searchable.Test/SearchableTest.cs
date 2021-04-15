using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Mallos.Searchable.Test
{
    class TestObject
    {
        public TestObject(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }

    class IsOne : IFilter<TestObject>
    {
        public string Key => "one";

        public IEnumerable<TestObject> Positive(IEnumerable<TestObject> query, string value)
            => query.Where(x => x.Value == "1");

        public IEnumerable<TestObject> Negative(IEnumerable<TestObject> query, string value)
            => query.Where(x => x.Value != "1");
    }

    class Value : IFilter<TestObject>
    {
        public string Key => "value";

        public IEnumerable<TestObject> Positive(IEnumerable<TestObject> query, string value)
            => query.Where(x => x.Value.Contains(value));

        public IEnumerable<TestObject> Negative(IEnumerable<TestObject> query, string value)
            => query.Where(x => !x.Value.Contains(value));
    }

    class TestObjectSearchable : Searchable<TestObject>
    {
        public TestObjectSearchable()
        {
            IsFilters.Add(new IsOne());
            Filters.Add(new Value());
        }

        protected override IEnumerable<TestObject> FreeTextFilter(IEnumerable<TestObject> values, bool negative, string text)
        {
            return base.FreeTextFilter(values, negative, text);
        }
    }

    public class SearchableTest
    {
        readonly TestObjectSearchable searchable = new TestObjectSearchable();
        readonly TestObject[] testdata = new TestObject[]
        {
            new ("1"),
            new ("2"),
            new ("3")
        };

        [Fact]
        public void Analyze_IsFilter_ValueMatch()
        {
            // Arrange
            var query = "is:one";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.False(analyzed[0].IsNegative);
            Assert.Equal("is", analyzed[0].Key);
            Assert.Equal("one", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_IsFilter_NegativeValueMatch()
        {
            // Arrange
            var query = "-is:one";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.True(analyzed[0].IsNegative);
            Assert.Equal("is", analyzed[0].Key);
            Assert.Equal("one", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_IsFilter_TableizedValueMatch()
        {
            // Arrange
            var query = "is:\"o-n-e\"";
            // NOTE: This have to be a string otherwise it wont recognize it.

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.False(analyzed[0].IsNegative);
            Assert.Equal("is", analyzed[0].Key);
            Assert.Equal("o-n-e", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_IsFilter_StringValueMatch()
        {
            // Arrange
            var query = "is:\"one\"";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.False(analyzed[0].IsNegative);
            Assert.Equal("is", analyzed[0].Key);
            Assert.Equal("one", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_Filter_ValueMatch()
        {
            // Arrange
            var query = "key:value";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.False(analyzed[0].IsNegative);
            Assert.Equal("key", analyzed[0].Key);
            Assert.Equal("value", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_Filter_NegativeValueMatch()
        {
            // Arrange
            var query = "-key:value";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.True(analyzed[0].IsNegative);
            Assert.Equal("key", analyzed[0].Key);
            Assert.Equal("value", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_FreeText_ValueChainMatch()
        {
            // Arrange
            var query = "my free text";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Equal(3, analyzed.Count);
            Assert.False(analyzed[0].IsNegative);
            Assert.False(analyzed[1].IsNegative);
            Assert.False(analyzed[2].IsNegative);
            Assert.Equal("my", analyzed[0].Value);
            Assert.Equal("free", analyzed[1].Value);
            Assert.Equal("text", analyzed[2].Value);
        }

        [Fact]
        public void Analyze_FreeText_StringValueMatch()
        {
            // Arrange
            var query = "\"my free text\"";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.Equal("my free text", analyzed[0].Value);
        }

        [Fact]
        public void Analyze_FreeText_NegativeValueMatch()
        {
            // Arrange
            var query = "-negative";

            // Act
            var analyzed = searchable.Analyze(query);

            // Assert
            Assert.Single(analyzed);
            Assert.True(analyzed[0].IsNegative);
            Assert.Equal("negative", analyzed[0].Value);
        }
    }
}
