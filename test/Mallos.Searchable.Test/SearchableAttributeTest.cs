namespace Mallos.Searchable.Test
{
    using System.Linq;
    using Xunit;

    public class SearchableAttributeTest
    {
        readonly TestObjectSearchable searchable = new();

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
    }
}
