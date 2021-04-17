namespace Mallos.Searchable.Test
{
    using Mallos.Searchable.Attributes;
    using System;

    [FilterGenerator("MyGeneratedSearchable")]
    public class TestObject
    {
        [FilterKey("v"), FilterStringMatch(FilterMatchType.Match)]
        [FilterKey("value"), FilterStringMatch(FilterMatchType.Contains, StringComparison.OrdinalIgnoreCase)]
        public string Value { get; set; }

        [FilterKey("key"), FilterStringMatch()]
        public string Key { get; set; }

        public TestObject(string value)
        {
            this.Value = value;
        }
    }
}
