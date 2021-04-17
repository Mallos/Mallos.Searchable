using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mallos.Searchable.CodeAnalyzer
{
    static class ReflectionHelper
    {
        public static IEnumerable<Type> AllOfType<T>()
        {
            return Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(T)));
        }
    }
}
