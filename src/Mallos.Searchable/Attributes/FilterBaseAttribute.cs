namespace Mallos.Searchable.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public abstract class FilterBaseAttribute : Attribute
    {
    }
}
