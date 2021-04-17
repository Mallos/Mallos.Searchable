namespace Mallos.Searchable.Attributes
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class FilterKeyAttribute : Attribute
    {
        public string Key { get; }

        public FilterKeyAttribute([CallerMemberName] string key = null)
        {
            this.Key = key;
        }
    }
}
