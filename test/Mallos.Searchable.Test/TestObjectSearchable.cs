namespace Mallos.Searchable.Test
{
    using System.Collections.Generic;
    using System.Linq;

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
}
