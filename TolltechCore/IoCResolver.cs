using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TolltechCore
{
    public static class IoCResolver
    {
        private static readonly HashSet<Type> emptyTypeHashset =  new HashSet<Type>();

        public static void Resolve(Action<Type, Type> resolve, HashSet<Type> ignoreTypes = null, params string[] assemblyNames)
        {

            ignoreTypes = ignoreTypes ?? emptyTypeHashset;

            var assemblyNameHashSet = new HashSet<string>(assemblyNames);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => assemblyNameHashSet.Any(y => x.FullName.StartsWith(y)))
                .ToArray();

            assemblies = assemblies
                .Concat(assemblies
                    .SelectMany(x => x.GetReferencedAssemblies())
                    .Where(x => assemblyNameHashSet.Any(y => x.FullName.StartsWith(y)))
                    .Select(Assembly.Load))
                .GroupBy(x => x.FullName)
                .Select(x => x.First())
                .ToArray();

            var interfaces = assemblies
                .SelectMany(x => x.GetTypes()
                    .Where(y => y.IsInterface))
                .Where(x => !ignoreTypes.Contains(x))
                .ToArray();

            var types = assemblies
                .SelectMany(x => x.GetTypes()
                    .Where(y => !y.IsInterface && y.IsClass && !y.IsAbstract))
                .Where(x => !ignoreTypes.Contains(x))
                .ToArray();

            foreach (var @interface in interfaces)
            {
                var realizations = types
                    .Where(x => @interface.IsAssignableFrom(x))
                    .ToArray();

                foreach (var realization in realizations)
                {
                    resolve(@interface, realization);
                }
            }
        }
    }
}