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
    }

    public class SearchableTest
    {
        [Fact]
        public void Test1()
        {
            var searchable = new TestObjectSearchable();


            var testdata = new TestObject[]
            {
                new ("1"),
                new ("2"),
                new ("3")
            };


            var analyzed = searchable.Analyze("is:one -is:one eric -eric value:1 -value:2");
            var result1 = searchable.Search(testdata, "is:one").ToList();
            var result2 = searchable.Search(testdata, "-is:one").ToList();
            var result3 = searchable.Search(testdata, "value:3").ToList();

            // TODO: Write unit tests
        }
    }
}
