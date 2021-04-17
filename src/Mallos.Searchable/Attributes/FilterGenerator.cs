namespace Mallos.Searchable.Attributes
{
    using System;

    // TODO: Add record for .NET 5
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class FilterGenerator : Attribute
    {
        public string ClassName { get; }

        public FilterGenerator(string className)
        {
            this.ClassName = className;
        }
    }
}
