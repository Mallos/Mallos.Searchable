namespace Mallos.Searchable.Test
{
    using System.Linq;
    using Xunit;

    public class SearchableTest
    {
        readonly TestObjectSearchable searchable = new TestObjectSearchable();

        [Fact]
        public void Search_IsFilter_FoundMatch()
        {
            // Arrange
            var query = "is:one";
            var values = new TestObject[]
            {
                new ("1"),
                new ("2"),
                new ("3"),
                new ("1 2")
            };

            // Act
            var result = searchable.Search(values, query).ToArray();

            // Assert
            Assert.Single(result);
            Assert.Equal(values[0].Value, result[0].Value);
        }

        [Fact]
        public void Search_IsFilter_NegativeFoundMatches()
        {
            // Arrange
            var query = "-is:one";
            var values = new TestObject[]
            {
                new ("1"),
                new ("2"),
                new ("3"),
                new ("1 2")
            };

            // Act
            var result = searchable.Search(values, query).ToArray();

            // Assert
            Assert.Equal(3, result.Length);
            Assert.Equal(values[1].Value, result[0].Value);
            Assert.Equal(values[2].Value, result[1].Value);
            Assert.Equal(values[3].Value, result[2].Value);
        }

        [Fact]
        public void Search_Filter_FoundMatches()
        {
            // Arrange
            var query = "value:1";
            var values = new TestObject[]
            {
                new ("1"),
                new ("2"),
                new ("3"),
                new ("1 2")
            };

            // Act
            var result = searchable.Search(values, query).ToArray();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(values[0].Value, result[0].Value);
            Assert.Equal(values[3].Value, result[1].Value);
        }

        [Fact]
        public void Search_Filter_NegativeFoundMatches()
        {
            // Arrange
            var query = "-value:1";
            var values = new TestObject[]
            {
                new ("1"),
                new ("2"),
                new ("3"),
                new ("1 2")
            };

            // Act
            var result = searchable.Search(values, query).ToArray();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(values[1].Value, result[0].Value);
            Assert.Equal(values[2].Value, result[1].Value);
        }
    }
}
